using Microsoft.EntityFrameworkCore;
using WSRS_Api.Data;
using WSRS_Api.Dtos;
using WSRS_Api.Interfaces;

namespace WSRS_Api.Repositories;

public class ViolationRepository : IViolationRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ViolationRepository> _logger;

    public ViolationRepository(ILogger<ViolationRepository> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IEnumerable<ReportEncodedDto>> GetByIdAsync(int studentNumber)
    {
        try
        {
            return await _context.ReportsEncoded
                .AsNoTracking()
                .Where(r => r.StudentNumber == studentNumber)
                    .Include(r => r.Offense)
                .Select(r => new ReportEncodedDto
                {
                    Id = r.Id,
                    OffenseId = r.OffenseId,
                    CommissionDate = r.CommissionDate,
                    Sanction = r.Sanction,
                    Description = r.Description
                })
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex.Message);
            return Enumerable.Empty<ReportEncodedDto>();
        }
    }
}
