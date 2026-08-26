namespace LogisticsPlatform.Extensions;

public static class RefreshCookie
{
    internal const string Name = "refreshToken";

    public static void Set(HttpResponse response, string refreshToken, DateTime expiresUtc)
    {
        response.Cookies.Append(Name, refreshToken, BuildOptions(response, expiresUtc));
    }

    public static void Clear(HttpResponse response)
    {
        response.Cookies.Delete(Name, BuildOptions(response, expiresUtc: null));
    }

    private static CookieOptions BuildOptions(HttpResponse response, DateTime? expiresUtc)
    {
        var environment = response.HttpContext.RequestServices
            .GetRequiredService<IHostEnvironment>();

        return new CookieOptions
        {
            HttpOnly = true,
            Secure = !environment.IsDevelopment(),
            SameSite = SameSiteMode.Lax,
            Expires = expiresUtc,
            Path = "/"
        };
    }
}
