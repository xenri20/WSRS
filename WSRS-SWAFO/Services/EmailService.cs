using DocumentFormat.OpenXml.Office2016.Presentation.Command;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using WSRS_SWAFO.Interfaces;
using WSRS_SWAFO.ViewModels;

namespace WSRS_SWAFO.Services
{
    public class EmailService : IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(EmailSubjectViewModel emailTemplate)
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
                Subject = "[#" + emailTemplate.id + "] Notice of Violation for " + emailTemplate.name + " - " + emailTemplate.sanction,
                Body = @"<!DOCTYPE html>
                        <html>
                        <head>
                        <meta charset=""UTF-8"">
                        <title>Violation Notice</title>
                        </head>
                        <body style=""font-family: 'Times New Roman', Times, serif; font-size: 12pt; line-height: 1.5; color: #000;"">
                        <p>Greetings from SWAFO,</p>

                        <p>We hope this message finds you well. This is to formally inform you that a violation has been recorded under your name. Kindly access your Violations Portal on the DLSU-D portal for more information.</p>

                        <p>Our office is committed to upholding student discipline and fairness. We encourage you to coordinate with us at the Student Welfare and Formation Office (SWAFO) at your earliest convenience to discuss the matter and address any concerns you may have.</p>

                        <p>Should you have any questions or need further clarification, please do not hesitate to visit our office at <strong>(GMH111)</strong>.</p>

                        <p>With Regards,<br>
                        <strong>Student Welfare and Formation Office (SWAFO)</strong></p>
                        </body>
                        </html>
                        ",
                IsBodyHtml = true
            };
            mailMessage.To.Add(emailTemplate.email);

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
