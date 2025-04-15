using WSRS_Api.Dtos;

namespace WSRS_Api.Repositories;

public class MockViolationRepository
{
    public Task<IEnumerable<ReportEncodedDto>> GetViolationsByStudentNumber(int studentNumber)
    {
        var mockData = new List<ReportEncodedDto>
        {
            new ReportEncodedDto
            {
                OffenseId = 101,
                CommissionDate = new DateOnly(2025, 1, 15),
                // HearingDate = new DateOnly(2025, 2, 1),
                Sanction = "Warning",
            },
            new ReportEncodedDto
            {
                OffenseId = 102,
                CommissionDate = new DateOnly(2025, 3, 10),
                // HearingDate = null, // Null for minor offenses
                Sanction = "Warning",
            }
        };

        return Task.FromResult(mockData.AsEnumerable());
    }
}
