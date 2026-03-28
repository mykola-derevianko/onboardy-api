using Microsoft.AspNetCore.Mvc;
using OnBoardy.API.DTOs;
using OnBoardy.API.Extensions;
using OnBoardy.API.Results;
using OnBoardy.API.Services.Infrastructure;

namespace OnBoardy.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ICookieService _cookieService;

        public AuthController(
            IAuthService authService,
            IConfiguration config,
            IWebHostEnvironment webHostEnvironment,
            ICookieService cookieService)
        {
            _authService = authService;
            _cookieService = cookieService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            if (result.IsFailure)
                return this.ToProblem(result.Error);

            return Ok(new { message = "Registration successful. Please verify your email." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var result = await _authService.LoginAsync(request, ip);

            if (result.IsFailure)
                return this.ToProblem(result.Error);

            _cookieService.SetAuthCookies(result.Value, HttpContext);
            return Ok(new { message = "Logged in successfully." });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refresh_token"];
            if (string.IsNullOrWhiteSpace(refreshToken))
                return this.ToProblem(AuthErrors.InvalidRefreshToken);

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var result = await _authService.RefreshAsync(refreshToken, ip);

            if (result.IsFailure)
                return this.ToProblem(result.Error);

            _cookieService.SetAuthCookies(result.Value, HttpContext);
            return Ok(new { message = "Refresh successful." });
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            var result = await _authService.VerifyEmailAsync(token);
            if (result.IsFailure)
                return this.ToProblem(result.Error);

            return Ok(new { message = "Email successfully verified." });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refresh_token"];
            var result = await _authService.LogoutAsync(refreshToken);

            if (result.IsFailure)
                return this.ToProblem(result.Error);

            _cookieService.ClearAuthCookies(HttpContext);
            return Ok(new { message = "Logged out successfully." });
        }
    }
}