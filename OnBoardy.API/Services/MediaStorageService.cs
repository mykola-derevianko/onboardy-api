using OnBoardy.API.Services.Infrastructure;

namespace OnBoardy.API.Services
{
    public class MediaStorageService : IMediaStorageService
    {
        private readonly IBlobService _blobService;

        public MediaStorageService(IBlobService blobService)
        {
            _blobService = blobService;
        }

        public string GenerateReadSas(string containerName, string blobName)
        {
            return _blobService.GenerateReadSas(containerName, blobName);
        }

        public async Task UploadAndReplaceAsync(
            string containerName,
            string newBlobName,
            Stream content,
            string contentType,
            string? oldBlobName,
            CancellationToken cancellationToken = default)
        {
            await _blobService.UploadAsync(
                containerName,
                newBlobName,
                content,
                contentType,
                cancellationToken);

            if (!string.IsNullOrWhiteSpace(oldBlobName))
            {
                await _blobService.DeleteAsync(containerName, oldBlobName);
            }
        }
    }
}