using Microsoft.EntityFrameworkCore;
using WSRS_Api.Data;
using WSRS_Api.Interfaces;
using WSRS_Api.Models;

namespace WSRS_Api.Repositories;

public class ViolationRepository : IViolationRepository
{
    private readonly ApplicationDbContext _context;

    public ViolationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ReportEncoded>> GetViolationsByStudentNumber(int studentNumber)
    {
        try
        {
            return await _context.ReportsEncoded
                .AsNoTracking()
                .Where(r => r.StudentNumber == studentNumber)
                    .Include(r => r.Offense)
                .ToListAsync();
        }
        catch
        {
            return Enumerable.Empty<ReportEncoded>();
        }
    }
}
