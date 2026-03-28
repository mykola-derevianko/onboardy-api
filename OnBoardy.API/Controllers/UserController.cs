using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnBoardy.API.Attributes;
using OnBoardy.API.Constants;
using OnBoardy.API.DTOs;
using OnBoardy.API.Exceptions.Domain;
using OnBoardy.API.Extensions;
using OnBoardy.API.Services.Infrastructure;

namespace OnBoardy.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapperService _mapperService;

        public UserController(IUserService userService, IMapperService mapperService)
        {
            _userService = userService;
            _mapperService = mapperService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create(RegisterRequest request)
        {
            var user = await _userService.CreateAsync(request);
            return CreatedAtAction(nameof(GetMe), _mapperService.ToUserResponse(user));
        }

        [HttpGet("me")]
        public async Task<ActionResult<UserResponse>> GetMe()
        {
            var currentUserId = User.GetUserId();

            var user = await _userService.GetByIdAsync(currentUserId)
                ?? throw new UserNotFoundException();

            return Ok(_mapperService.ToUserResponse(user));
        }

        [HttpPatch("me")]
        public async Task<ActionResult<UserResponse>> UpdateMe(UpdateUserRequest request)
        {
            var currentUserId = User.GetUserId();

            var user = await _userService.UpdateAsync(currentUserId, request);
            return Ok(_mapperService.ToUserResponse(user));
        }

        [HttpDelete("me")]
        public async Task<IActionResult> DeleteMe()
        {
            var currentUserId = User.GetUserId();

            await _userService.DeleteAsync(currentUserId);
            return NoContent();
        }

        [HttpPost("me/profile-picture")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(MediaValidation.MaxProfilePictureBytes)]
        [RequestFormLimits(MultipartBodyLengthLimit = MediaValidation.MaxProfilePictureBytes)]
        public async Task<IActionResult> UploadProfilePicture(
            [AllowedImageFile(MediaValidation.MaxProfilePictureBytes)] IFormFile file,
            CancellationToken cancellationToken)
        {
            await using var stream = file.OpenReadStream();

            var userId = User.GetUserId();
            await _userService.SaveProfilePictureAsync(
                userId,
                stream,
                file.FileName,
                file.ContentType,
                cancellationToken);

            return Ok(new { message = "Profile picture updated successfully." });
        }
    }
}