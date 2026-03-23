using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnBoardy.API.Constants;
using OnBoardy.API.DTOs;
using OnBoardy.API.Enums;
using OnBoardy.API.Exceptions.Domain;
using OnBoardy.API.Extensions;
using OnBoardy.API.Models;
using OnBoardy.API.Services.Infrastructure;

namespace OnBoardy.API.Controllers
{
    [ApiController]
    [Route("api/organizations")]
    [Authorize]
    public class OrganizationController : ControllerBase
    {
        private const long MaxLogoBytes = 2 * 1024 * 1024;
        private const long MaxBannerBytes = 5 * 1024 * 1024;

        private static readonly HashSet<string> AllowedContentTypes =
        [
            "image/png",
            "image/jpg",
            "image/jpeg",
            "image/webp"
        ];

        private readonly IOrganizationService _organizationService;
        private readonly IBlobService _blobService;

        public OrganizationController(
            IOrganizationService organizationService,
            IBlobService blobService)
        {
            _organizationService = organizationService;
            _blobService = blobService;
        }

        [HttpPost]
        public async Task<ActionResult<OrganizationResponse>> Create(CreateOrganizationRequest request)
        {
            var currentUserId = User.GetUserId();

            var organization = await _organizationService.CreateAsync(request, currentUserId);
            return CreatedAtAction(nameof(GetById), new { orgId = organization.Id }, organization.ToResponseDTO());
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyCollection<OrganizationResponse>>> GetMyOrganizations(
            [FromQuery] IEnumerable<MembershipRole>? membershipRoles)
        {
            var currentUserId = User.GetUserId();

            var organizations = await _organizationService.GetAllByUserIdAsync(currentUserId, membershipRoles);

            return Ok(organizations.Select(ToResponseWithMediaUrls).ToList());
        }

        [HttpGet("{orgId:guid}")]
        public async Task<ActionResult<OrganizationResponse>> GetById(Guid orgId)
        {
            var currentUserId = User.GetUserId();

            var organizations = await _organizationService.GetAllByUserIdAsync(currentUserId);
            var organization = organizations.FirstOrDefault(x => x.Id == orgId)
                ?? throw new OrganizationNotFoundException();

            return Ok(ToResponseWithMediaUrls(organization));
        }

        private OrganizationResponse ToResponseWithMediaUrls(Organization organization)
        {
            var response = organization.ToResponseDTO();

            if (!string.IsNullOrWhiteSpace(organization.LogoBlobName))
            {
                var logoUrl = _blobService.GenerateReadSas(
                    BlobContainers.OrganizationMedia,
                    organization.LogoBlobName);

                response = response with { LogoUrl = logoUrl };
            }

            if (!string.IsNullOrWhiteSpace(organization.BannerBlobName))
            {
                var bannerUrl = _blobService.GenerateReadSas(
                    BlobContainers.OrganizationMedia,
                    organization.BannerBlobName);

                response = response with { BannerUrl = bannerUrl };
            }

            return response;
        }

        [HttpPatch("{orgId:guid}")]
        [TypeFilter(
            typeof(AuthorizeRoleFilter),
            Arguments = new object[] { new[] { MembershipRole.Owner } }
        )]
        public async Task<ActionResult<OrganizationResponse>> Update(Guid orgId, UpdateOrganizationRequest request)
        {
            var organization = await _organizationService.UpdateAsync(orgId, request);
            return Ok(organization.ToResponseDTO());
        }

        [HttpDelete("{orgId:guid}")]
        [TypeFilter(
            typeof(AuthorizeRoleFilter),
            Arguments = new object[] { new[] { MembershipRole.Owner } }
        )]
        public async Task<IActionResult> Delete(Guid orgId)
        {
            await _organizationService.DeleteAsync(orgId);
            return NoContent();
        }

        [HttpPost("{orgId:guid}/logo")]
        [TypeFilter(
            typeof(AuthorizeRoleFilter),
            Arguments = new object[] { new[] { MembershipRole.Owner } }
        )]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(MaxLogoBytes)]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxLogoBytes)]
        public async Task<IActionResult> UploadLogo(
            Guid orgId,
            IFormFile file,
            CancellationToken cancellationToken)
        {
            if (file is null || file.Length == 0)
                throw new DomainException("File is required.");

            if (file.Length > MaxLogoBytes)
                return StatusCode(StatusCodes.Status413PayloadTooLarge);

            var contentType = file.ContentType?.ToLowerInvariant() ?? string.Empty;
            if (!AllowedContentTypes.Contains(contentType))
                return BadRequest("Invalid file type");

            await using var stream = file.OpenReadStream();

            await _organizationService.SaveLogoAsync(
                orgId,
                stream,
                file.FileName,
                contentType,
                cancellationToken);

            return Ok(new { message = "Organization logo updated successfully." });
        }

        [HttpPost("{orgId:guid}/banner")]
        [TypeFilter(
            typeof(AuthorizeRoleFilter),
            Arguments = new object[] { new[] { MembershipRole.Owner } }
        )]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(MaxBannerBytes)]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxBannerBytes)]
        public async Task<IActionResult> UploadBanner(
            Guid orgId,
            IFormFile file,
            CancellationToken cancellationToken)
        {
            if (file is null || file.Length == 0)
                throw new DomainException("File is required.");

            if (file.Length > MaxBannerBytes)
                return StatusCode(StatusCodes.Status413PayloadTooLarge);

            var contentType = file.ContentType?.ToLowerInvariant() ?? string.Empty;
            if (!AllowedContentTypes.Contains(contentType))
                return BadRequest("Invalid file type");

            await using var stream = file.OpenReadStream();

            await _organizationService.SaveBannerAsync(
                orgId,
                stream,
                file.FileName,
                contentType,
                cancellationToken);

            return Ok(new { message = "Organization banner updated successfully." });
        }
    }
}