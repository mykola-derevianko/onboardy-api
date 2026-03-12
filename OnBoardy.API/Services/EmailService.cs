using OnBoardy.API.Services.Infrastructure;
using System.Net;
using System.Net.Mail;

namespace OnBoardy.API.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPass;
        private readonly string _from;

        public EmailService(IConfiguration config)
        {
            _smtpHost = config["SMTP:Host"]
                ?? throw new ArgumentNullException("SMTP:Host configuration is missing");
            _smtpPort = int.TryParse(config["SMTP:Port"], out var port) ? port : 587;
            _smtpUser = config["SMTP:Username"]
                ?? throw new ArgumentNullException("SMTP:Username configuration is missing");
            _smtpPass = config["SMTP:Password"]
                ?? throw new ArgumentNullException("SMTP:Password configuration is missing");
            _from = config["SMTP:SenderEmail"]
                ?? throw new ArgumentNullException("SMTP:From configuration is missing");
        }

        public async Task SendAsync(string to, string subject, string html)
        {
            using var client = new SmtpClient(_smtpHost, _smtpPort)
            {
                Credentials = new NetworkCredential(_smtpUser, _smtpPass),
                EnableSsl = true
            };

            using var mail = new MailMessage
            {
                From = new MailAddress(_from),
                Subject = subject,
                Body = html,
                IsBodyHtml = true
            };

            mail.To.Add(to);

            await client.SendMailAsync(mail);
        }
    }
}