namespace OnBoardy.API.Results
{
    public sealed record Error(string Code, string Message, int StatusCode)
    {
        public static readonly Error None = new(string.Empty, string.Empty, StatusCodes.Status200OK);
    }
}