using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using OnBoardy.API.DTOs;
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

        public UploadSasResponse GenerateUploadSas(
            string containerName, Guid userId, string extension)
        {
            var container = GetContainerClient(containerName);

            var blobName = $"{userId}/{Guid.NewGuid()}{extension}";

            var blobClient = container.GetBlobClient(blobName);

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = container.Name,
                BlobName = blobName,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(10)
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Write);

            var token = sasBuilder.ToSasQueryParameters(
                new StorageSharedKeyCredential(_accountName, _accountKey));

            var uploadUrl = $"{blobClient.Uri}?{token}";

            return new UploadSasResponse
            {
                UploadUrl = uploadUrl,
                BlobName = blobName
            };
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

        public async Task DeleteAsync(string containerName, string blobName)
        {
            if (string.IsNullOrWhiteSpace(blobName))
                return;

            var container = GetContainerClient(containerName);
            await container.DeleteBlobIfExistsAsync(blobName);
        }
    }
}
