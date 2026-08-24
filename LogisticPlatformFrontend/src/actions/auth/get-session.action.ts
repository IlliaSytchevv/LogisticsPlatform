"use server";

import { cookies } from "next/headers";
import { ACCESS_TOKEN_COOKIE } from "@/lib/auth/constants";
import { parseAuthUser } from "@/lib/auth/token";
import type { AuthUser } from "@/types/auth";

export async function getSessionAction(): Promise<AuthUser | null> {
  const jar = await cookies();
  const access = jar.get(ACCESS_TOKEN_COOKIE)?.value;
  if (!access) return null;
  return parseAuthUser(access);
}
