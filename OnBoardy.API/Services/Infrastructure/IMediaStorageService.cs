namespace OnBoardy.API.Services.Infrastructure
{
    public interface IMediaStorageService
    {
        string GenerateReadSas(string containerName, string blobName);

        Task UploadAndReplaceAsync(
            string containerName,
            string newBlobName,
            Stream content,
            string contentType,
            string? oldBlobName,
            CancellationToken cancellationToken = default);
    }
}