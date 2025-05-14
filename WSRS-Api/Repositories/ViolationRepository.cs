using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
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

    public async Task<AllReportsDto> GetByIdAsync(int studentNumber)
    {
        try
        {
            var student = await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.StudentNumber == studentNumber);

            var violations = await _context.ReportsEncoded
                .AsNoTracking()
                .Where(r => r.StudentNumber == studentNumber)
                    .Include(r => r.Offense)
                .Select(r => new ReportEncodedDto
                {
                    Offense = new OffenseDto
                    {
                        Classification = r.Offense.Classification.GetDisplayName(),
                        Nature = r.Offense.Nature,
                    },
                    Description = r.Description,
                    CommissionDate = r.CommissionDate,
                    Sanction = r.Sanction,
                })
                .ToListAsync();

            var trafficViolations = await _context.TrafficReportsEncoded
                .AsNoTracking()
                .Where(tr => tr.StudentNumber == studentNumber)
                    .Include(tr => tr.Offense)
                .Select(tr => new TrafficReportEncodedDto
                {
                    Offense = new OffenseDto
                    {
                        Classification = tr.Offense.Classification.GetDisplayName(),
                        Nature = tr.Offense.Nature,
                    },
                    CommissionDate = tr.CommissionDate,
                    Remarks = tr.Remarks,
                })
                .ToListAsync();

            return new AllReportsDto
            {
                Student = student!,
                Violations = violations,
                TrafficViolations = trafficViolations
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex.Message);
        }

        // return an empty list just in case something goes wrong
        return new AllReportsDto
        {
            Student = {},
            Violations = [],
            TrafficViolations = []
        };
    }

    public async Task<bool> IsStudentClear(int studentNumber)
    {
        bool result = true;

        try
        {
            var violations = await _context.ReportsEncoded
                .AsNoTracking()
                .Where(r => r.StudentNumber == studentNumber)
                .Select(r => new ReportEncodedDto
                {
                    Offense = new OffenseDto
                    {
                        Classification = r.Offense.Classification.GetDisplayName(),
                        Nature = r.Offense.Nature,
                    },
                    StatusOfSanction = r.StatusOfSanction
                })
                .ToListAsync();

            var trafficViolations = await _context.TrafficReportsEncoded
                .AsNoTracking()
                .Where(tr => tr.StudentNumber == studentNumber)
                .Select(tr => new TrafficReportEncodedDto
                {
                    Offense = new OffenseDto
                    {
                        Classification = tr.Offense.Classification.GetDisplayName(),
                        Nature = tr.Offense.Nature,
                    },
                    DatePaid = tr.DatePaid,
                })
                .ToListAsync();

            if (violations.Any())
            {
                if (violations.Any(v => v.StatusOfSanction != "Completed"))
                {
                    result = false;
                    return await Task.FromResult(result);
                }
            }

            if (trafficViolations.Any())
            {
                if (trafficViolations.Any(tv => tv.DatePaid == null))
                {
                    result = false;
                    return await Task.FromResult(result);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }

        return await Task.FromResult(result);
    }
}
