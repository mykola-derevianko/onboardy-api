using OnBoardy.API.DTOs;

namespace OnBoardy.API.Services.Infrastructure
{
    public interface IBlobService
    {
        string GenerateReadSas(string containerName, string blobName);

        Task UploadAsync(
            string containerName,
            string blobName,
            Stream content,
            string contentType,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(string containerName, string blobName);
    }
}