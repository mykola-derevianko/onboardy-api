namespace OnBoardy.API.DTOs
{
    public record UploadSasResponse
    {
        public required string UploadUrl { get; init; }
        public required string BlobName { get; init; }
    }
}
