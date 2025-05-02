using System.Net.Mail;

namespace WSRS_SWAFO.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
