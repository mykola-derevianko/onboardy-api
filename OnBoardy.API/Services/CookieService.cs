using OnBoardy.API.DTOs;
using OnBoardy.API.Services.Infrastructure;

namespace OnBoardy.API.Services
{
    public class CookieService : ICookieService
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;

        public CookieService(IConfiguration config, IWebHostEnvironment env)
        {
            _config = config;
            _env = env;
        }

        public void SetAuthCookies(TokenDTO token, HttpContext context)
        {
            var accessTokenOptions = CreateCookieOptions(DateTime.UtcNow.AddMinutes(
                double.TryParse(_config["Jwt:ExpireMinutes"], out double expireMinutes) ? expireMinutes : 60));

            var refreshTokenOptions = CreateCookieOptions(DateTime.UtcNow.AddDays(
                double.TryParse(_config["Jwt:RefreshTokenExpireDays"], out double refreshTokenExpireDays) ? refreshTokenExpireDays : 7));

            context.Response.Cookies.Append("access_token", token.AccessToken, accessTokenOptions);
            context.Response.Cookies.Append("refresh_token", token.RefreshToken, refreshTokenOptions);
        }

        public void ClearAuthCookies(HttpContext context)
        {
            var expiredOptions = CreateCookieOptions(DateTime.UtcNow.AddDays(-1));

            context.Response.Cookies.Append("access_token", string.Empty, expiredOptions);
            context.Response.Cookies.Append("refresh_token", string.Empty, expiredOptions);
        }

        private CookieOptions CreateCookieOptions(DateTime expires)
        {
            bool isDevelopment = _env.IsDevelopment();

            return new CookieOptions
            {
                HttpOnly = true,
                Secure = !isDevelopment,
                SameSite = isDevelopment ? SameSiteMode.Lax : SameSiteMode.None,
                Domain = isDevelopment ? null : _config["Cookie:Domain"],
                Expires = expires
            };
        }
    }
}
