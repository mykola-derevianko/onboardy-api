namespace OnBoardy.API.Services.Infrastructure
{
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string html);
    }
}
