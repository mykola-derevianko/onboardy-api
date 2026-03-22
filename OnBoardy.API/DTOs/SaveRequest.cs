namespace OnBoardy.API.DTOs
{
    public record SaveRequest
    {
        public required string BlobName { get; init; }
    }
}
