using WSRS_Api.Dtos;
using WSRS_Api.Interfaces;

namespace WSRS_Api.Repositories;

public class MockViolationRepository
{
    public Task<IEnumerable<ReportEncodedDto>> GetViolationsByStudentNumber(int studentNumber)
    {
        var mockData = new List<ReportEncodedDto>
        {
            new ReportEncodedDto
            {
                Id = 1,
                OffenseId = 101,
                CommissionDate = new DateOnly(2025, 1, 15),
                // HearingDate = new DateOnly(2025, 2, 1),
                Sanction = "Warning",
                Description = "Late submission of assignment",
            },
            new ReportEncodedDto
            {
                Id = 2,
                OffenseId = 102,
                CommissionDate = new DateOnly(2025, 3, 10),
                // HearingDate = null, // Null for minor offenses
                Sanction = "Warning",
                Description = "Missed class without notice",
            }
        };

        return Task.FromResult(mockData.AsEnumerable());
    }
}
