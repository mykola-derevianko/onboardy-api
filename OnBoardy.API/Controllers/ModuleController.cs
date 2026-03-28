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
using System.Net;

namespace OnBoardy.API.Controllers
{
    [ApiController]
    [Route("api/organizations/{orgId:guid}/modules")]
    [Authorize]
    public class ModuleController : ControllerBase
    {
        private readonly IModuleService _moduleService;
        private readonly IBlobService _blobService;

        public ModuleController(IModuleService moduleService, IBlobService blobService)
        {
            _moduleService = moduleService;
            _blobService = blobService;
        }

        [HttpPost]
        [TypeFilter(
            typeof(AuthorizeRoleFilter),
            Arguments = new object[] { new[] { MembershipRole.Owner, MembershipRole.Admin } }
        )]
        public async Task<ActionResult<ModuleResponse>> Create(Guid orgId, CreateModuleRequest request)
        {
            var currentUserId = User.GetUserId();

            var module = await _moduleService.CreateAsync(request, orgId, currentUserId);
            var response = ToResponseDTO(module!);

            return CreatedAtAction(
                nameof(GetById),
                new { orgId, moduleId = response.Id },
                response);
        }

        [HttpPatch("{moduleId:guid}")]
        [TypeFilter(
            typeof(AuthorizeRoleFilter),
            Arguments = new object[] { new[] { MembershipRole.Owner, MembershipRole.Admin } }
        )]
        public async Task<ActionResult<ModuleResponse>> Update(Guid orgId, Guid moduleId, UpdateModuleRequest request)
        {
            _ = orgId;

            var currentUserId = User.GetUserId();
            var module = await _moduleService.UpdateAsync(moduleId, request, currentUserId);

            if (module is null)
                throw new DomainException("Module not found.", HttpStatusCode.NotFound);

            return Ok(ToResponseDTO(module));
        }   

        [HttpDelete("{moduleId:guid}")]
        [TypeFilter(
            typeof(AuthorizeRoleFilter),
            Arguments = new object[] { new[] { MembershipRole.Owner, MembershipRole.Admin } }
        )]
        public async Task<IActionResult> Delete(Guid orgId, Guid moduleId)
        {
            _ = orgId;

            await _moduleService.DeleteAsync(moduleId);
            return NoContent();
        }

        [HttpPost("{moduleId:guid}/banner")]
        [TypeFilter(
            typeof(AuthorizeRoleFilter),
            Arguments = new object[] { new[] { MembershipRole.Owner, MembershipRole.Admin } }
        )]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(MediaValidation.MaxOrganizationBannerBytes)]
        [RequestFormLimits(MultipartBodyLengthLimit = MediaValidation.MaxOrganizationBannerBytes)]
        public async Task<IActionResult> UploadBanner(
            Guid orgId,
            Guid moduleId,
            [AllowedImageFile(MediaValidation.MaxOrganizationBannerBytes)] IFormFile file,
            CancellationToken cancellationToken)
        {
            _ = orgId;

            await using var stream = file.OpenReadStream();

            await _moduleService.SaveBannerAsync(
                moduleId,
                stream,
                file.FileName,
                file.ContentType,
                cancellationToken);

            return Ok(new { message = "Module banner updated successfully." });
        }

        [HttpGet("{moduleId:guid}")]
        [TypeFilter(
            typeof(AuthorizeRoleFilter),
            Arguments = new object[] { new[] { MembershipRole.Owner, MembershipRole.Admin, MembershipRole.Employee } }
        )]
        public async Task<ActionResult<ModuleResponse>> GetById(Guid orgId, Guid moduleId)
        {
            var module = await _moduleService.GetByIdAsync(moduleId);

            if (module is null || module.OrganizationId != orgId)
                throw new DomainException("Module not found.", HttpStatusCode.NotFound);

            return Ok(ToResponseDTO(module));
        }

        private ModuleResponse ToResponseDTO(Module module)
        {
            var bannerBlobUrl = string.IsNullOrWhiteSpace(module.BannerBlobName)
                ? null
                : _blobService.GenerateReadSas(BlobContainers.OrganizationMedia, module.BannerBlobName);

            return new ModuleResponse
            {
                Id = module.Id,
                Name = module.Name,
                Description = module.Description,
                Status = module.Status,
                OrganizationId = module.OrganizationId,
                CreatedBy = module.CreatedBy,
                CreatedAt = module.CreatedAt,
                UpdatedAt = module.UpdatedAt,
                BannerBlobUrl = bannerBlobUrl
            };
        }
    }
}