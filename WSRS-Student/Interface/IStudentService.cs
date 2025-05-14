using System.Security.Claims;

namespace WSRS_Student.Interface
{
    public interface IStudentService
    {
        Task<bool> HandleStudentStatus(ClaimsPrincipal User);
    }
}