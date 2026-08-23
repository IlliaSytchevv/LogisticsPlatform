import { decodeJwt } from "jose";
import { AUTH_COOKIE_NAME } from "@/lib/auth/constants";
import type { AuthUser } from "@/types/auth";

const NAME_ID =
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
const NAME = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
const ROLE = "http://schemas.microsoft.com/ws/2008/05/identity/claims/role";

export function parseAuthUser(token: string): AuthUser | null {
  try {
    const payload = decodeJwt(token);
    const roles = payload[ROLE];
    return {
      id: String(payload[NAME_ID] ?? payload.sub ?? ""),
      name: String(payload[NAME] ?? payload.unique_name ?? ""),
      roles: Array.isArray(roles)
        ? roles.map(String)
        : roles
          ? [String(roles)]
          : [],
    };
  } catch {
    return null;
  }
}

export function readTokenFromCookieHeader(cookieHeader: string | null): string | null {
  if (!cookieHeader) return null;
  const match = cookieHeader
    .split("; ")
    .find((row) => row.startsWith(`${AUTH_COOKIE_NAME}=`));
  return match ? decodeURIComponent(match.split("=")[1] ?? "") : null;
}
