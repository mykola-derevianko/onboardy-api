namespace OnBoardy.API.DTOs
{
    public record UploadRequest
    {
        public required string FileName { get; init; }
    }
}
