"use server";

import { cookies } from "next/headers";
import {
  ACCESS_TOKEN_COOKIE,
  ACCESS_TOKEN_MAX_AGE,
  BACKEND_REFRESH_COOKIE,
  REFRESH_TOKEN_COOKIE,
  REFRESH_TOKEN_MAX_AGE,
  authCookieOptions,
} from "@/lib/auth/constants";
import {
  buildCookieHeader,
  extractCookieValue,
  readSetCookieHeaders,
} from "@/lib/auth/cookie-utils";
import { apiV1 } from "@/lib/api/routes";
import { API_BASE_URL } from "@/lib/api/base-url";

export async function refreshAccessTokenAction(): Promise<boolean> {
  const jar = await cookies();
  const refresh = jar.get(REFRESH_TOKEN_COOKIE)?.value;
  if (!refresh) return false;

  try {
    const response = await fetch(`${API_BASE_URL}${apiV1("/auth/refresh-token")}`, {
      method: "POST",
      headers: {
        Cookie: buildCookieHeader({ [BACKEND_REFRESH_COOKIE]: refresh }),
      },
      cache: "no-store",
    });

    if (!response.ok) return false;

    const body = (await response.json()) as { jwtToken?: string };
    if (!body.jwtToken) return false;

    const setCookies = readSetCookieHeaders(response);
    const newRefresh =
      extractCookieValue(setCookies, BACKEND_REFRESH_COOKIE) ??
      extractCookieValue(setCookies, REFRESH_TOKEN_COOKIE);

    jar.set(
      ACCESS_TOKEN_COOKIE,
      body.jwtToken,
      authCookieOptions(ACCESS_TOKEN_MAX_AGE),
    );
    if (newRefresh) {
      jar.set(
        REFRESH_TOKEN_COOKIE,
        newRefresh,
        authCookieOptions(REFRESH_TOKEN_MAX_AGE),
      );
    }

    return true;
  } catch {
    return false;
  }
}
