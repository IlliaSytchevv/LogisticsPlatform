/** Parse a cookie value from one or more Set-Cookie header strings. */
export function extractCookieValue(
  setCookieHeaders: string[] | string | null | undefined,
  cookieName: string,
): string | null {
  if (!setCookieHeaders) return null;

  const headers = Array.isArray(setCookieHeaders)
    ? setCookieHeaders
    : [setCookieHeaders];

  for (const header of headers) {
    const match = header.match(new RegExp(`(?:^|,\\s*)${cookieName}=([^;]+)`));
    if (match?.[1]) return decodeURIComponent(match[1].trim());

    // Single Set-Cookie line: name=value; Path=...
    if (header.startsWith(`${cookieName}=`)) {
      return decodeURIComponent(header.slice(cookieName.length + 1).split(";")[0].trim());
    }
  }

  return null;
}

export function readSetCookieHeaders(response: Response): string[] {
  const anyHeaders = response.headers as Headers & {
    getSetCookie?: () => string[];
  };
  if (typeof anyHeaders.getSetCookie === "function") {
    return anyHeaders.getSetCookie();
  }

  const raw = response.headers.get("set-cookie");
  return raw ? [raw] : [];
}

export function buildCookieHeader(parts: Record<string, string | undefined | null>): string {
  return Object.entries(parts)
    .filter(([, v]) => Boolean(v))
    .map(([k, v]) => `${k}=${v}`)
    .join("; ");
}
