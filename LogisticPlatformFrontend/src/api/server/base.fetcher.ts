import { cookies } from "next/headers";
import { refreshAccessTokenOnce } from "@/api/server/refresh-mutex";
import {
  ACCESS_TOKEN_COOKIE,
  BACKEND_REFRESH_COOKIE,
  REFRESH_TOKEN_COOKIE,
} from "@/lib/auth/constants";
import { buildCookieHeader } from "@/lib/auth/cookie-utils";
import { ApiError } from "@/types/auth";

const API_URL = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5124";

type ServerFetcherOptions = RequestInit & {
  skipAuth?: boolean;
  skipRefresh?: boolean;
};

async function readAccessToken(): Promise<string | null> {
  const jar = await cookies();
  return jar.get(ACCESS_TOKEN_COOKIE)?.value ?? null;
}

async function readRefreshToken(): Promise<string | null> {
  const jar = await cookies();
  return jar.get(REFRESH_TOKEN_COOKIE)?.value ?? null;
}

/**
 * Server-side API client: attaches Bearer access_token from httpOnly cookies.
 * On 401, refreshes once (deduped) and retries the original request.
 */
export async function serverFetcher<T>(
  path: string,
  options: ServerFetcherOptions = {},
): Promise<T> {
  const { skipAuth, skipRefresh, headers, ...init } = options;

  const access = skipAuth ? null : await readAccessToken();
  const response = await fetch(`${API_URL}${path}`, {
    ...init,
    headers: {
      ...(init.body instanceof FormData ? {} : { "Content-Type": "application/json" }),
      ...(access ? { Authorization: `Bearer ${access}` } : {}),
      ...headers,
    },
    cache: "no-store",
  });

  if (response.status === 401 && !skipAuth && !skipRefresh) {
    const refresh = await readRefreshToken();
    if (refresh) {
      const ok = await refreshAccessTokenOnce(refresh);
      if (ok) {
        return serverFetcher<T>(path, { ...options, skipRefresh: true });
      }
    }
  }

  if (!response.ok) {
    const raw = await response.text();
    let body: unknown = raw;
    try {
      body = raw ? JSON.parse(raw) : null;
    } catch {
      // keep raw
    }
    throw new ApiError(
      typeof body === "string" && body ? body : `HTTP ${response.status}`,
      response.status,
      body,
    );
  }

  if (response.status === 204) return undefined as T;

  const contentType = response.headers.get("content-type") ?? "";
  if (!contentType.includes("application/json")) {
    return undefined as T;
  }

  return (await response.json()) as T;
}

export async function serverFetchRaw(
  path: string,
  init: RequestInit = {},
  options: { skipRefresh?: boolean } = {},
): Promise<Response> {
  const access = await readAccessToken();
  const response = await fetch(`${API_URL}${path}`, {
    ...init,
    headers: {
      ...(access ? { Authorization: `Bearer ${access}` } : {}),
      ...init.headers,
    },
    cache: "no-store",
  });

  if (response.status === 401 && !options.skipRefresh) {
    const refresh = await readRefreshToken();
    if (refresh) {
      const ok = await refreshAccessTokenOnce(refresh);
      if (ok) {
        return serverFetchRaw(path, init, { skipRefresh: true });
      }
    }
  }

  return response;
}

/** Forward backend refresh cookie when calling auth endpoints that expect it. */
export async function backendRefreshCookieHeader(): Promise<string | undefined> {
  const refresh = await readRefreshToken();
  if (!refresh) return undefined;
  return buildCookieHeader({ [BACKEND_REFRESH_COOKIE]: refresh });
}
