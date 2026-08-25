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
  extractCookieValue,
  readSetCookieHeaders,
} from "@/lib/auth/cookie-utils";
import { parseAuthUser } from "@/lib/auth/token";
import type { AuthUser } from "@/types/auth";

const API_URL = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5124";

export type LoginActionResult =
  | { ok: true; user: AuthUser | null }
  | { ok: false; message: string; status?: number };

export async function loginAction(
  username: string,
  password: string,
): Promise<LoginActionResult> {
  const response = await fetch(`${API_URL}/api/auth/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ username, password }),
    cache: "no-store",
  });

  if (!response.ok) {
    const raw = await response.text();
    return {
      ok: false,
      status: response.status,
      message:
        response.status === 401
          ? "Wrong username or password."
          : raw || `HTTP ${response.status}`,
    };
  }

  const body = (await response.json()) as { jwtToken?: string; token?: string };
  const access = body.jwtToken ?? body.token;
  if (!access) {
    return { ok: false, message: "Login response missing jwtToken." };
  }

  const setCookies = readSetCookieHeaders(response);
  const refresh =
    extractCookieValue(setCookies, BACKEND_REFRESH_COOKIE) ??
    extractCookieValue(setCookies, REFRESH_TOKEN_COOKIE);

  const jar = await cookies();
  jar.set(ACCESS_TOKEN_COOKIE, access, authCookieOptions(ACCESS_TOKEN_MAX_AGE));
  if (refresh) {
    jar.set(
      REFRESH_TOKEN_COOKIE,
      refresh,
      authCookieOptions(REFRESH_TOKEN_MAX_AGE),
    );
  }

  return { ok: true, user: parseAuthUser(access) };
}
