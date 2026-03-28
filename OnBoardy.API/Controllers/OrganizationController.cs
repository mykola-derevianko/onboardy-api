using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnBoardy.API.Attributes;
using OnBoardy.API.Constants;
using OnBoardy.API.DTOs;
using OnBoardy.API.Enums;
using OnBoardy.API.Exceptions.Domain;
using OnBoardy.API.Extensions;
using OnBoardy.API.Services.Infrastructure;

namespace OnBoardy.API.Controllers
{
    [ApiController]
    [Route("api/organizations")]
    [Authorize]
    public class OrganizationController : ControllerBase
    {
        private readonly IOrganizationService _organizationService;
        private readonly IMapperService _mapperService;

        public OrganizationController(
            IOrganizationService organizationService,
            IMapperService mapperService)
        {
            _organizationService = organizationService;
            _mapperService = mapperService;
        }

        [HttpPost]
        public async Task<ActionResult<OrganizationResponse>> Create(CreateOrganizationRequest request)
        {
            var currentUserId = User.GetUserId();

            var organization = await _organizationService.CreateAsync(request, currentUserId);
            return CreatedAtAction(nameof(GetById), new { orgId = organization.Id }, _mapperService.ToOrganizationResponse(organization));
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyCollection<OrganizationResponse>>> GetMyOrganizations(
            [FromQuery] IEnumerable<MembershipRole>? membershipRoles)
        {
            var currentUserId = User.GetUserId();

            var organizations = await _organizationService.GetAllByUserIdAsync(currentUserId, membershipRoles);

            return Ok(organizations.Select(_mapperService.ToOrganizationResponse).ToList());
        }

        [HttpGet("{orgId:guid}")]
        public async Task<ActionResult<OrganizationResponse>> GetById(Guid orgId)
        {
            var currentUserId = User.GetUserId();

            var organizations = await _organizationService.GetAllByUserIdAsync(currentUserId);
            var organization = organizations.FirstOrDefault(x => x.Id == orgId)
                ?? throw new OrganizationNotFoundException();

            return Ok(_mapperService.ToOrganizationResponse(organization));
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
    }
}