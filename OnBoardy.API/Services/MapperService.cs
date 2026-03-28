using OnBoardy.API.Constants;
using OnBoardy.API.DTOs;
using OnBoardy.API.Models;
using OnBoardy.API.Services.Infrastructure;

namespace OnBoardy.API.Services
{
    public class MapperService : IMapperService
    {
        private readonly IMediaStorageService _mediaStorageService;

        public MapperService(IMediaStorageService mediaStorageService)
        {
            _mediaStorageService = mediaStorageService;
        }

        public UserResponse ToUserResponse(User user)
        {
            var response = new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsActive = user.IsActive,
                EmailVerified = user.EmailVerified,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                ProfilePictureUrl = null
            };

            if (!string.IsNullOrWhiteSpace(user.ProfilePictureBlobName))
            {
                response = response with
                {
                    ProfilePictureUrl = _mediaStorageService.GenerateReadSas(
                        BlobContainers.ProfilePictures,
                        user.ProfilePictureBlobName)
                };
            }

            return response;
        }

        public OrganizationResponse ToOrganizationResponse(Organization organization)
        {
            var response = new OrganizationResponse
            {
                Id = organization.Id,
                Name = organization.Name,
                Description = organization.Description,
                CreatedAt = organization.CreatedAt,
                UpdatedAt = organization.UpdatedAt,
                LogoUrl = null,
                BannerUrl = null
            };

            if (!string.IsNullOrWhiteSpace(organization.LogoBlobName))
            {
                response = response with
                {
                    LogoUrl = _mediaStorageService.GenerateReadSas(
                        BlobContainers.OrganizationMedia,
                        organization.LogoBlobName)
                };
            }

            if (!string.IsNullOrWhiteSpace(organization.BannerBlobName))
            {
                response = response with
                {
                    BannerUrl = _mediaStorageService.GenerateReadSas(
                        BlobContainers.OrganizationMedia,
                        organization.BannerBlobName)
                };
            }

            return response;
        }

        public ModuleResponse ToModuleResponse(Module module)
        {
            var bannerBlobUrl = string.IsNullOrWhiteSpace(module.BannerBlobName)
                ? null
                : _mediaStorageService.GenerateReadSas(
                    BlobContainers.OrganizationMedia,
                    module.BannerBlobName);

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