export const ACCESS_TOKEN_COOKIE =
  process.env.ACCESS_TOKEN_COOKIE ?? "access_token";

export const REFRESH_TOKEN_COOKIE =
  process.env.REFRESH_TOKEN_COOKIE ?? "refresh_token";

/** Cookie name set by the ASP.NET API on login/refresh. */
export const BACKEND_REFRESH_COOKIE = "refreshToken";

/** @deprecated use ACCESS_TOKEN_COOKIE */
export const AUTH_COOKIE_NAME = ACCESS_TOKEN_COOKIE;

export const ACCESS_TOKEN_MAX_AGE = 60 * 35; // 35 min
export const REFRESH_TOKEN_MAX_AGE = 60 * 60 * 24 * 7; // 7 days

export function authCookieOptions(maxAge: number) {
  return {
    httpOnly: true as const,
    sameSite: "lax" as const,
    secure: process.env.NODE_ENV === "production",
    path: "/",
    maxAge,
  };
}
