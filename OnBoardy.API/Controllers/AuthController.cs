using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

            _cookieService.SetAuthCookies(result, HttpContext);
            return Ok(new { message = "Logged in successfully." });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refresh_token"] ?? throw new UnauthorizedAccessException();
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var result = await _authService.RefreshAsync(refreshToken, ip);
            _cookieService.SetAuthCookies(result, HttpContext);
            return Ok(new { message = "Refresh successful." });
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            await _authService.VerifyEmailAsync(token);
            return Ok(new { message = "Email successfully verified." });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refresh_token"];

            await _authService.LogoutAsync(refreshToken);
            _cookieService.ClearAuthCookies(HttpContext);
            return Ok(new { message = "Logged out successfully." });
        }

    }
}