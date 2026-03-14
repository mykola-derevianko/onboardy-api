using OnBoardy.API.Models;

namespace OnBoardy.API.Services.Infrastructure
{
    public interface IEmailVerificationService
    {
        Task SendVerificationEmailAsync(User user);
        Task<EmailVerification> ValidateTokenAsync(string token);
        Task MarkAsVerifiedAsync(EmailVerification record);
    }
}