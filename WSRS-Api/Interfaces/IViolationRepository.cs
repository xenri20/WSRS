using WSRS_Api.Dtos;

namespace WSRS_Api.Interfaces;

public interface IViolationRepository
{
    Task<IEnumerable<ReportEncodedDto>> GetByIdAsync(int studentNumber);
}
