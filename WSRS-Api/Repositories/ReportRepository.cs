using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.JsonPatch;
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

        public IEnumerable<ReportsPending> GetAll()
        {
            try
            {
                var reports = _context.ReportsPending.ToList();
                return reports;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return new List<ReportsPending>();
        }

        public ReportsPending? GetById(int id)
        {
            try
            {
                var report = _context.ReportsPending.Find(id);
                if (report != null) return report;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return null;
        }

        public ReportPendingDto? PostStudentViolation(ReportPendingDto reportDto)
        {
            try
            {
                // Model validation
                var validationContext = new ValidationContext(reportDto);
                var validationResults = new List<ValidationResult>();

                if (!Validator.TryValidateObject(reportDto, validationContext, validationResults, true))
                {
                    foreach (var validationResult in validationResults)
                    {
                        _logger.LogWarning(validationResult.ErrorMessage);
                    }
                    throw new ArgumentException("The provided report is invalid.");
                }

                var reportPending = new ReportsPending
                {
                    FormatorId = reportDto.FormatorId,
                    Description = reportDto.Description,
                    StudentNumber = reportDto.StudentNumber,
                    ReportDate = reportDto.ReportDate,
                    Formator = reportDto.Formator,
                    FirstName = reportDto.FirstName,
                    LastName = reportDto.LastName,
                    College = reportDto.College,
                    CourseYearSection = reportDto.CourseYearSection,
                    IsArchived = false
                };

                _context.ReportsPending.Add(reportPending);
                _context.SaveChanges();
                return reportDto;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
            return null;
        }
    }
}
