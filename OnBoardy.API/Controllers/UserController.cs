using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IBlobService _blobService;

        public UserController(IUserService userService, IBlobService blobService)
        {
            _userService = userService;
            _blobService = blobService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create(RegisterRequest request)
        {
            var user = await _userService.CreateAsync(request);
            return CreatedAtAction(nameof(GetMe), user.ToResponseDTO());
        }

        [HttpGet("me")]
        public async Task<ActionResult<UserResponse>> GetMe()
        {
            var currentUserId = User.GetUserId();

            var user = await _userService.GetByIdAsync(currentUserId)
                ?? throw new UserNotFoundException();

            var response = user.ToResponseDTO();

            if (!string.IsNullOrWhiteSpace(user.ProfilePictureBlobName))
            {
                var readUrl = _blobService.GenerateReadSas(
                    BlobContainers.ProfilePictures,
                    user.ProfilePictureBlobName);

                response = response with { ProfilePictureUrl = readUrl };
            }

            return Ok(response);
        }

        [HttpPatch("me")]
        public async Task<ActionResult<UserResponse>> UpdateMe(UpdateUserRequest request)
        {
            var currentUserId = User.GetUserId();

            var user = await _userService.UpdateAsync(currentUserId, request);
            return Ok(user.ToResponseDTO());
        }

        [HttpDelete("me")]
        public async Task<IActionResult> DeleteMe()
        {
            var currentUserId = User.GetUserId();

            await _userService.DeleteAsync(currentUserId);
            return NoContent();
        }

        [HttpPost("me/profile-picture/upload-url")]
        public IActionResult GetUploadUrl([FromBody] UploadRequest request)
        {
            var currentUserId = User.GetUserId();

            var result = _userService.GenerateProfilePictureUpload(currentUserId, request.FileName);

            return Ok(result);
        }

        [HttpPost("me/profile-picture")]
        public async Task<IActionResult> SaveProfilePicture([FromBody] SaveRequest request)
        {
            var userId = User.GetUserId();

            await _userService.SaveProfilePictureAsync(userId, request.BlobName);

            return Ok();
        }
    }
}