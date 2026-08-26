"use server";

import { cookies } from "next/headers";
import {
  ACCESS_TOKEN_COOKIE,
  BACKEND_REFRESH_COOKIE,
  REFRESH_TOKEN_COOKIE,
} from "@/lib/auth/constants";
import { buildCookieHeader } from "@/lib/auth/cookie-utils";
import { apiV1 } from "@/lib/api/routes";
import { API_BASE_URL } from "@/lib/api/base-url";

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
      await fetch(`${API_BASE_URL}${apiV1("/auth/logout")}`, {
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
