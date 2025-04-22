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

        public ReportsPendingDto? PostStudentViolation(int FormatorId, string Description, int StudentNumber, string Formator)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Description))
                {
                    throw new ArgumentException("Description cannot be null or empty.", nameof(Description));
                }

                if (string.IsNullOrWhiteSpace(Formator))
                {
                    throw new ArgumentException("Formator cannot be null or empty.", nameof(Formator));
                }

                if (StudentNumber <= 0)
                {
                    throw new ArgumentException("StudentNumber must be greater than zero.", nameof(StudentNumber));
                }

                var reportPending = new ReportsPending
                {
                    FormatorId = FormatorId,
                    Description = Description,
                    StudentNumber = StudentNumber,
                    ReportDate = DateOnly.FromDateTime(DateTime.Now),
                    Formator = Formator,
                    IsArchived = false
                };

                _context.ReportsPending.Add(reportPending);
                _context.SaveChanges();

                var reportPendingDto = new ReportsPendingDto
                {
                    FormatorId = reportPending.FormatorId,
                    Description = reportPending.Description,
                    StudentNumber = reportPending.StudentNumber,
                    ReportDate = reportPending.ReportDate,
                    Formator = reportPending.Formator,
                    IsArchived = reportPending.IsArchived
                };

                return reportPendingDto;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return null;
        }
    }
}
