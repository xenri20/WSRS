using WSRS_Api.Models;

namespace WSRS_Api.Interfaces;

public interface IViolationRepository
{
    Task<IEnumerable<ReportEncoded>> GetViolationsByStudentNumber(int studentNumber);

}
