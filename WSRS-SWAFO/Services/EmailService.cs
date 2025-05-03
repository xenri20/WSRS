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
                Body = Message(emailTemplate.emailMode),
                IsBodyHtml = true
            };
            mailMessage.To.Add(emailTemplate.email);

            await client.SendMailAsync(mailMessage);
        }

        public string Message(int emailMode)
        {
            if (emailMode == 0)
            {
                return @"<!DOCTYPE html>
                        <html>
                        <head>
                        <meta charset=""UTF-8"">
                        <title>Violation Notice</title>
                        </head>
                        <body style=""font-family: 'Times New Roman', Times, serif; font-size: 12pt; line-height: 1.5; color: #000;"">
                        <p>Greetings from SWAFO,</p>

                        <p>We hope this message finds you well.</p>

                        <p>This is to respectfully inform you that a violation has been recorded under your name. For more details, kindly log in to your Violations Portal via the DLSU-D portal.</p>

                        <p>Our office remains committed to promoting student discipline and fairness. We encourage you to coordinate with the Student Welfare and Formation Office (SWAFO) at your earliest convenience to discuss the matter or raise any concerns you may have.</p>

                        <p>If you need any assistance or clarification, please feel free to visit our office at <strong>GMH111</strong>.</p>

                        <p style=""font-style: italic; color: #555;"">This is an automated message. Please do not reply to this email.</p>

                        <p>With sincere regards,<br>
                        <strong>Student Welfare and Formation Office (SWAFO)</strong></p>
                        </body>
                        </html>";
            }

            if (emailMode == 1)
            {
                return @"<!DOCTYPE html>
                        <html>
                        <head>
                        <meta charset=""UTF-8"">
                        <title>Scheduled Hearing Notification</title>
                        </head>
                        <body style=""font-family: 'Times New Roman', Times, serif; font-size: 12pt; line-height: 1.5; color: #000;"">
                        <p>Greetings from SWAFO,</p>

                        <p>We hope this message finds you well.</p>

                        <p>This is to respectfully inform you that a hearing has been scheduled in relation to a major violation recorded under your name. You are required to attend the hearing at <strong>GMH111</strong> on the scheduled date and time, as part of the due process.</p>

                        <p>We kindly remind you to arrive on time and come prepared for the discussion. Please note that failure to attend without valid justification may lead to further disciplinary action in accordance with university policies.</p>

                        <p>If you have any questions or concerns, feel free to visit the Student Welfare and Formation Office (SWAFO) for assistance.</p>

                        <p style=""font-style: italic; color: #555;"">This is an automated message. Please do not reply to this email.</p>

                        <p>With sincere regards,<br>
                        <strong>Student Welfare and Formation Office (SWAFO)</strong></p>
                        </body>
                        </html>";
            }

            return "";
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
