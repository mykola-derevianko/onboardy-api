using Microsoft.AspNetCore.Mvc;
using OnBoardy.API.DTOs;
using OnBoardy.API.Exceptions.Identity;
using OnBoardy.API.Services.Infrastructure;

namespace OnBoardy.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _config;


        public AuthController(IAuthService authService, IConfiguration config)
        {
            _authService = authService;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDTO request)
        {
            await _authService.RegisterAsync(request);
            return Ok(new { message = "Registration successful. Please verify your email." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDTO request)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var result = await _authService.LoginAsync(request, ip);

            SetTokensInHttpOnlyCookies(result, HttpContext);
            return Ok(new { message = "Logged in successfully." });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refresh_token"] ?? throw new UnauthorizedAccessException();
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var result = await _authService.RefreshAsync(refreshToken, ip);
            return Ok(new { message = "Refresh successful." });
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            await _authService.VerifyEmailAsync(token);
            return Ok(new { message = "Email successfully verified." });
        }

        private void SetTokensInHttpOnlyCookies(TokenDTO token, HttpContext context)
        {
            if (!double.TryParse(_config["Jwt:ExpireMinutes"], out double expireMinutes))
                expireMinutes = 60;
            if (!double.TryParse(_config["Jwt:RefreshTokenExpireDays"], out double refreshTokenExpireDays))
                refreshTokenExpireDays = 7;

            var accessTokenOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes)
            };
            var refreshTokenOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(refreshTokenExpireDays)
            };
            context.Response.Cookies.Append("access_token", token.AccessToken, accessTokenOptions);
            context.Response.Cookies.Append("refresh_token", token.RefreshToken, refreshTokenOptions);
        }

    }
}