"use server";

import { cookies } from "next/headers";
import {
  ACCESS_TOKEN_COOKIE,
  BACKEND_REFRESH_COOKIE,
  REFRESH_TOKEN_COOKIE,
} from "@/lib/auth/constants";
import { buildCookieHeader } from "@/lib/auth/cookie-utils";

const API_URL = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5124";

async function clearAuthCookies() {
  const jar = await cookies();
  jar.delete(ACCESS_TOKEN_COOKIE);
  jar.delete(REFRESH_TOKEN_COOKIE);
}

export async function logoutAction() {
  const jar = await cookies();
  const refresh = jar.get(REFRESH_TOKEN_COOKIE)?.value;

  try {
    if (refresh) {
      await fetch(`${API_URL}/api/auth/logout`, {
        method: "POST",
        headers: {
          Cookie: buildCookieHeader({ [BACKEND_REFRESH_COOKIE]: refresh }),
        },
        cache: "no-store",
      });
    }
  } catch {
    // ignore backend errors — always clear local cookies
  } finally {
    await clearAuthCookies();
  }
}

export async function clearAuthCookiesAction() {
  await clearAuthCookies();
}
