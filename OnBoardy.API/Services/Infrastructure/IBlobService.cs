using OnBoardy.API.DTOs;

namespace OnBoardy.API.Services.Infrastructure
{
    public interface IBlobService
    {
        UploadSasResponse GenerateUploadSas(
            string containerName, Guid userId, string extension);

        string GenerateReadSas(string containerName, string blobName);

        Task DeleteAsync(string containerName, string blobName);
    }
}
