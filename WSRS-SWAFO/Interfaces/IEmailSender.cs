using System.Net.Mail;
using WSRS_SWAFO.ViewModels;

namespace WSRS_SWAFO.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(EmailSubjectViewModel emailTemplate);
    }
}
