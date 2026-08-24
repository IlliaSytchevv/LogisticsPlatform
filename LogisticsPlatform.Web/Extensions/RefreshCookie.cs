namespace LogisticsPlatform.Extensions;

public static class RefreshCookie
{
    internal const string Name = "refreshToken";

    public static void Set(HttpResponse response, string refreshToken, DateTime expiresUtc)
    {
        response.Cookies.Append(Name, refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = expiresUtc,
            Path = "/"
        });
    }

    public static void Clear(HttpResponse response)
    {
        response.Cookies.Delete(Name, new CookieOptions { Path = "/" });
    }
}