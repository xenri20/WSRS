using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WSRS_Api.Data;
using WSRS_Api.Dtos;
using WSRS_Api.Models;

namespace WSRS_Api.Repositories
{
    public class ReportRepository
    {
        private ApplicationDbContext _context;
        private readonly ILogger<ReportRepository> _logger;

        public ReportRepository(ILogger<ReportRepository> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public Task<ReportPendingDto> PostStudentViolation(string FormatorId, string Description, int StudentNumber)
        {
            var reportPending = new ReportPendingDto
                {
                    FormatorId = FormatorId,
                    Description = Description,
                    CommissionDatetime = DateTime.Now,
                    StudentNumber = StudentNumber
                };

                _context.ReportPending.Any(reportPending);
                _context.SaveChanges();
        }
    }
}
