using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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

        public ActionResult<ReportPending> PostStudentViolation(string FormatorId, string Description, int StudentNumber)
        {
            try
            {
                var reportPending = new ReportPending
                {
                    FormatorId = FormatorId,
                    Description = Description,
                    CommissionDatetime = DateTime.Now,
                    StudentNumber = StudentNumber
                };

                _context.ReportPending.Add(reportPending);
                _context.SaveChanges();

                return new OkObjectResult(reportPending);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }

            return null;
        }
    }
}
