import { decodeJwt } from "jose";
import { ACCESS_TOKEN_COOKIE } from "@/lib/auth/constants";
import type { AuthUser } from "@/types/auth";

const NAME_ID =
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
const NAME = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
const ROLE = "http://schemas.microsoft.com/ws/2008/05/identity/claims/role";

export function parseAuthUser(token: string): AuthUser | null {
  try {
    const payload = decodeJwt(token);
    if (typeof payload.exp === "number" && payload.exp * 1000 <= Date.now()) {
      return null;
    }

    const typ = payload.typ ?? payload["typ"];
    if (typ === "refresh") return null;

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

export function isAccessTokenExpired(token: string): boolean {
  try {
    const payload = decodeJwt(token);
    if (typeof payload.exp !== "number") return true;
    // small skew
    return payload.exp * 1000 <= Date.now() + 5_000;
  } catch {
    return true;
  }
}

export function readTokenFromCookieHeader(cookieHeader: string | null): string | null {
  if (!cookieHeader) return null;
  const match = cookieHeader
    .split("; ")
    .find((row) => row.startsWith(`${ACCESS_TOKEN_COOKIE}=`));
  return match ? decodeURIComponent(match.split("=")[1] ?? "") : null;
}
