"use server";

import { cookies } from "next/headers";
import { redirect } from "next/navigation";
import { AUTH_COOKIE_NAME } from "@/lib/auth/constants";

const COOKIE_MAX_AGE = 60 * 60;

export async function setAuthToken(token: string) {
  const jar = await cookies();
  // Readable by client fetcher (Bearer). Fine for local/small apps; BFF later if needed.
  jar.set(AUTH_COOKIE_NAME, token, {
    httpOnly: false,
    sameSite: "lax",
    secure: process.env.NODE_ENV === "production",
    path: "/",
    maxAge: COOKIE_MAX_AGE,
  });
}

export async function clearAuthToken() {
  const jar = await cookies();
  jar.delete(AUTH_COOKIE_NAME);
}

export async function logoutAction() {
  await clearAuthToken();
  redirect("/login");
}
