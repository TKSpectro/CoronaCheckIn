using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;

namespace CoronaCheckIn.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public EmailSender(ILogger<EmailSender> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var smtp = new SmtpClient(_configuration["smtp:host"]);
            smtp.EnableSsl = bool.Parse(_configuration["smtp:enableSsl"]);
            smtp.Port = int.Parse(_configuration["smtp:port"]);
            smtp.Credentials =
                new NetworkCredential(_configuration["smtp:auth:user"], _configuration["smtp:auth:pass"]);

            return smtp.SendMailAsync("auth@coronacheckin.xyz", email, subject, htmlMessage);
        }
    }
}