using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using OnBoardy.API.Services.Infrastructure;

namespace OnBoardy.API.Services
{
    public class BlobService : IBlobService
    {
        private readonly string _connectionString;
        private readonly string _accountName;
        private readonly string _accountKey;

        public BlobService(IConfiguration config)
        {
            _connectionString = config["AzureBlob:ConnectionString"]
                ?? throw new ArgumentNullException("AzureBlob:ConnectionString");
            _accountName = config["AzureBlob:AccountName"]
                ?? throw new ArgumentNullException("AzureBlob:AccountName");
            _accountKey = config["AzureBlob:AccountKey"]
                ?? throw new ArgumentNullException("AzureBlob:AccountKey");
        }

        private BlobContainerClient GetContainerClient(string containerName)
        {
            var client = new BlobServiceClient(_connectionString);
            return client.GetBlobContainerClient(containerName);
        }

        public string GenerateReadSas(string containerName, string blobName)
        {
            var container = GetContainerClient(containerName);
            var blobClient = container.GetBlobClient(blobName);

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = container.Name,
                BlobName = blobName,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(30)
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            var token = sasBuilder.ToSasQueryParameters(
                new StorageSharedKeyCredential(_accountName, _accountKey));

            return $"{blobClient.Uri}?{token}";
        }

        public async Task UploadAsync(
            string containerName,
            string blobName,
            Stream content,
            string contentType,
            CancellationToken cancellationToken = default)
        {
            var container = GetContainerClient(containerName);
            await container.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

            var blobClient = container.GetBlobClient(blobName);

            var options = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = string.IsNullOrWhiteSpace(contentType)
                        ? "application/octet-stream"
                        : contentType
                }
            };

            await blobClient.UploadAsync(content, options, cancellationToken);
        }

        public async Task DeleteAsync(string containerName, string blobName)
        {
            if (string.IsNullOrWhiteSpace(blobName))
                return;

            var container = GetContainerClient(containerName);
            await container.DeleteBlobIfExistsAsync(blobName);
        }
    }
}
