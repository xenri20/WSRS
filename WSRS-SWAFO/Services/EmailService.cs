using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using WSRS_SWAFO.Interfaces;

namespace WSRS_SWAFO.Services
{
    public class EmailService : IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            using var client = new SmtpClient(_emailSettings.SmtpClientMail, _emailSettings.PortAddress)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_emailSettings.DevEmail, _emailSettings.DevPass)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.DevEmail),
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);

            await client.SendMailAsync(mailMessage);
        }
    }

    public class EmailSettings
    {
        public string SmtpClientMail { get; set; }
        public int PortAddress { get; set; }
        public string DevEmail { get; set; }
        public string DevPass { get; set; }
    }
}
