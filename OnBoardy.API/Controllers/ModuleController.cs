using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnBoardy.API.Attributes;
using OnBoardy.API.Constants;
using OnBoardy.API.DTOs;
using OnBoardy.API.Enums;
using OnBoardy.API.Extensions;
using OnBoardy.API.Results;
using OnBoardy.API.Services.Infrastructure;

namespace OnBoardy.API.Controllers
{
    [ApiController]
    [Route("api/organizations/{orgId:guid}/modules")]
    [Authorize]
    public class ModuleController : ControllerBase
    {
        private readonly IModuleService _moduleService;
        private readonly IMapperService _mapperService;

        public ModuleController(IModuleService moduleService, IMapperService mapperService)
        {
            _moduleService = moduleService;
            _mapperService = mapperService;
        }

        [HttpPost]
        [TypeFilter(typeof(AuthorizeRoleFilter), Arguments = new object[] { new[] { MembershipRole.Owner, MembershipRole.Admin } })]
        public async Task<ActionResult<ModuleResponse>> Create(Guid orgId, CreateModuleRequest request)
        {
            var currentUserId = User.GetUserId();

            var result = await _moduleService.CreateAsync(request, orgId, currentUserId);
            if (result.IsFailure)
                return this.ToProblem(result.Error);

            var response = _mapperService.ToModuleResponse(result.Value);

            return CreatedAtAction(
                nameof(GetById),
                new { orgId, moduleId = response.Id },
                response);
        }

        [HttpPatch("{moduleId:guid}")]
        [TypeFilter(typeof(AuthorizeRoleFilter), Arguments = new object[] { new[] { MembershipRole.Owner, MembershipRole.Admin } })]
        public async Task<ActionResult<ModuleResponse>> Update(Guid orgId, Guid moduleId, UpdateModuleRequest request)
        {
            var currentUserId = User.GetUserId();
            var result = await _moduleService.UpdateAsync(moduleId, request, currentUserId);

            if (result.IsFailure)
                return this.ToProblem(result.Error);

            if (result.Value.OrganizationId != orgId)
                return this.ToProblem(ModuleErrors.NotFound);

            return Ok(_mapperService.ToModuleResponse(result.Value));
        }

        [HttpDelete("{moduleId:guid}")]
        [TypeFilter(typeof(AuthorizeRoleFilter), Arguments = new object[] { new[] { MembershipRole.Owner, MembershipRole.Admin } })]
        public async Task<IActionResult> Delete(Guid orgId, Guid moduleId)
        {
            var moduleResult = await _moduleService.GetByIdAsync(moduleId);
            if (moduleResult.IsFailure)
                return this.ToProblem(moduleResult.Error);

            if (moduleResult.Value.OrganizationId != orgId)
                return this.ToProblem(ModuleErrors.NotFound);

            var result = await _moduleService.DeleteAsync(moduleId);
            if (result.IsFailure)
                return this.ToProblem(result.Error);

            return NoContent();
        }

        [HttpPost("{moduleId:guid}/banner")]
        [TypeFilter(typeof(AuthorizeRoleFilter), Arguments = new object[] { new[] { MembershipRole.Owner, MembershipRole.Admin } })]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(MediaValidation.MaxOrganizationBannerBytes)]
        [RequestFormLimits(MultipartBodyLengthLimit = MediaValidation.MaxOrganizationBannerBytes)]
        public async Task<IActionResult> UploadBanner(
            Guid orgId,
            Guid moduleId,
            [AllowedImageFile(MediaValidation.MaxOrganizationBannerBytes)] IFormFile file,
            CancellationToken cancellationToken)
        {
            var moduleResult = await _moduleService.GetByIdAsync(moduleId);
            if (moduleResult.IsFailure)
                return this.ToProblem(moduleResult.Error);

            if (moduleResult.Value.OrganizationId != orgId)
                return this.ToProblem(ModuleErrors.NotFound);

            await using var stream = file.OpenReadStream();

            var result = await _moduleService.SaveBannerAsync(
                moduleId,
                stream,
                file.FileName,
                file.ContentType,
                cancellationToken);

            if (result.IsFailure)
                return this.ToProblem(result.Error);

            return Ok(new { message = "Module banner updated successfully." });
        }

        [HttpGet("{moduleId:guid}")]
        [TypeFilter(typeof(AuthorizeRoleFilter), Arguments = new object[] { new[] { MembershipRole.Owner, MembershipRole.Admin, MembershipRole.Employee } })]
        public async Task<ActionResult<ModuleResponse>> GetById(Guid orgId, Guid moduleId)
        {
            var result = await _moduleService.GetByIdAsync(moduleId);
            if (result.IsFailure)
                return this.ToProblem(result.Error);

            if (result.Value.OrganizationId != orgId)
                return this.ToProblem(ModuleErrors.NotFound);

            return Ok(_mapperService.ToModuleResponse(result.Value));
        }
    }
}