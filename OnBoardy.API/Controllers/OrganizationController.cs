using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnBoardy.API.Attributes;
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
        [RequestSizeLimit(MediaValidation.MaxOrganizationLogoBytes)]
        [RequestFormLimits(MultipartBodyLengthLimit = MediaValidation.MaxOrganizationLogoBytes)]
        public async Task<IActionResult> UploadLogo(
            Guid orgId,
            [AllowedImageFile(MediaValidation.MaxOrganizationLogoBytes)] IFormFile file,
            CancellationToken cancellationToken)
        {
            await using var stream = file.OpenReadStream();

            await _organizationService.SaveMediaAsync(
                orgId,
                stream,
                file.FileName,
                file.ContentType,
                OrganizationMediaType.Logo,
                cancellationToken);

            return Ok(new { message = "Organization logo updated successfully." });
        }

        [HttpPost("{orgId:guid}/banner")]
        [TypeFilter(
            typeof(AuthorizeRoleFilter),
            Arguments = new object[] { new[] { MembershipRole.Owner } }
        )]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(MediaValidation.MaxOrganizationBannerBytes)]
        [RequestFormLimits(MultipartBodyLengthLimit = MediaValidation.MaxOrganizationBannerBytes)]
        public async Task<IActionResult> UploadBanner(
            Guid orgId,
            [AllowedImageFile(MediaValidation.MaxOrganizationBannerBytes)] IFormFile file,
            CancellationToken cancellationToken)
        {
            await using var stream = file.OpenReadStream();

            await _organizationService.SaveMediaAsync(
                orgId,
                stream,
                file.FileName,
                file.ContentType,
                OrganizationMediaType.Banner,
                cancellationToken);

            return Ok(new { message = "Organization banner updated successfully." });
        }


        //TODO: Refactor to avoid code duplication with UserController's media URL generation.
        // Move maps to a separate service and inject it where needed?
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
    }
}