using WSRS_Api.Interfaces;
using WSRS_Api.Models;

namespace WSRS_Api.Repositories;

public class MockViolationRepository : IViolationRepository
{
    public Task<IEnumerable<ReportEncoded>> GetViolationsByStudentNumber(int studentNumber)
    {
        var mockData = new List<ReportEncoded>
        {
            new ReportEncoded
            {
                Id = 1,
                OffenseId = 101,
                CommissionDate = new DateOnly(2025, 1, 15),
                HearingDate = new DateOnly(2025, 2, 1),
                Sanction = "Warning",
                Description = "Late submission of assignment",
            },
            new ReportEncoded
            {
                Id = 2,
                OffenseId = 102,
                CommissionDate = new DateOnly(2025, 3, 10),
                HearingDate = null, // Null for minor offenses
                Sanction = "Warning",
                Description = "Missed class without notice",
            }
        };

        return Task.FromResult(mockData.AsEnumerable());
    }
}
