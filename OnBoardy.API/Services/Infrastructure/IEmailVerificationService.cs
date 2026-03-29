using OnBoardy.API.Models;
using OnBoardy.API.Results;

namespace OnBoardy.API.Services.Infrastructure
{
    public interface IEmailVerificationService
    {
        Task SendVerificationEmailAsync(User user);
        Task<Result<EmailVerification>> ValidateTokenAsync(string token);
        Task<Result> MarkAsVerifiedAsync(EmailVerification record);
    }
}