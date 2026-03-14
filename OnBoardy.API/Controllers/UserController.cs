using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnBoardy.API.DTOs;
using OnBoardy.API.Exceptions.Domain;
using OnBoardy.API.Exceptions.Identity;
using OnBoardy.API.Models;
using OnBoardy.API.Services.Infrastructure;

namespace OnBoardy.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create(RegisterRequestDTO request)
        {
            var user = await _userService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, Map(user));
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<UserResponseDTO>> GetById(Guid id)
        {
            //Method declared for further use
            EnsureSelfAccess(id);

            var user = await _userService.GetByIdAsync(id)
                ?? throw new UserNotFoundException();

            return Ok(Map(user));
        }

        [HttpPatch("{id:guid}")]
        public async Task<ActionResult<UserResponseDTO>> Update(Guid id, UpdateUserRequestDTO request)
        {
            EnsureSelfAccess(id);

            var user = await _userService.UpdateAsync(id, request);
            return Ok(Map(user));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            EnsureSelfAccess(id);

            await _userService.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("me")]
        public async Task<ActionResult<UserResponseDTO>> GetMe()
        {
            var currentUserId = GetCurrentUserId();

            var user = await _userService.GetByIdAsync(currentUserId)
                ?? throw new UserNotFoundException();

            return Ok(Map(user));
        }

        private Guid GetCurrentUserId()
        {
            var subject =
                User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(subject, out var userId))
                throw new InvalidUserContextException();

            return userId;
        }

        private void EnsureSelfAccess(Guid targetUserId)
        {
            var currentUserId = GetCurrentUserId();

            if (currentUserId != targetUserId)
                throw new InsufficientPermissionsException();
        }

        private static UserResponseDTO Map(User user)
        {
            return new UserResponseDTO
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsActive = user.IsActive,
                EmailVerified = user.EmailVerified,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }
    }
}