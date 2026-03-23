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
        private const long MaxProfilePictureBytes = 2 * 1024 * 1024;

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

        [HttpPost("me/profile-picture")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(MaxProfilePictureBytes)]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxProfilePictureBytes)]
        public async Task<IActionResult> UploadProfilePicture(IFormFile file, CancellationToken cancellationToken)
        {
            if (file is null || file.Length == 0)
                throw new DomainException("File is required.");

            var allowedContentTypes = new[] { "image/png", "image/jpg", "image/jpeg", "image/webp" };

            if (!allowedContentTypes.Contains(file.ContentType.ToLowerInvariant()))
                return BadRequest("Invalid file type");

            if (file.Length > MaxProfilePictureBytes)
                return StatusCode(StatusCodes.Status413PayloadTooLarge);

            await using var stream = file.OpenReadStream();

            var userId = User.GetUserId();
            await _userService.SaveProfilePictureAsync(
                userId,
                stream,
                file.FileName,
                file.ContentType ?? "application/octet-stream",
                cancellationToken);

            return Ok(new { message = "Profile picture updated successfully." });
        }
    }
}