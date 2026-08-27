import { ApiError } from "@/types/auth";
import { formatApiErrorMessage } from "@/lib/api/errors";

type FetcherOptions = RequestInit & {
  skipAuth?: boolean;
};

/**
 * Browser API client — goes through Next.js BFF (`/api/backend/*`)
 * so httpOnly access/refresh cookies stay on the Next origin.
 * Refresh-on-401 happens inside the BFF route (server).
 */
export async function fetcher<T>(
  path: string,
  options: FetcherOptions = {},
): Promise<T> {
  const { skipAuth: _skipAuth, headers, ...init } = options;

  const response = await fetch(`/api/backend${path}`, {
    ...init,
    credentials: "include",
    headers: {
      ...(init.body instanceof FormData
        ? {}
        : { "Content-Type": "application/json" }),
      ...headers,
    },
  });

  if (!response.ok) {
    const raw = await response.text();
    let body: unknown = raw;
    try {
      body = raw ? JSON.parse(raw) : null;
    } catch {
      // keep raw
    }
    throw new ApiError(formatApiErrorMessage(body, response.status), response.status, body);
  }

  if (response.status === 204) return undefined as T;

  const contentType = response.headers.get("content-type") ?? "";
  if (!contentType.includes("application/json")) {
    return undefined as T;
  }

  return (await response.json()) as T;
}

export function toQuery(params: Record<string, unknown>): string {
  const search = new URLSearchParams();
  for (const [key, value] of Object.entries(params)) {
    if (value === undefined || value === null || value === "") continue;
    search.set(key, String(value));
  }
  const qs = search.toString();
  return qs ? `?${qs}` : "";
}
