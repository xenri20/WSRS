using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WSRS_SWAFO.Data;
using WSRS_SWAFO.ViewModels;
using WSRS_SWAFO.Interfaces;

namespace WSRS_SWAFO.Controllers
{
    public class HearingSchedulingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<HearingSchedulingController> _logger;

        public HearingSchedulingController(ApplicationDbContext context, IEmailSender emailSender, ILogger<HearingSchedulingController> logger)
        {
            _context = context;
            _emailSender = emailSender;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetStudent(int studentNumber)
        {
            var student = _context.Students.FirstOrDefault(s => s.StudentNumber == studentNumber);
            if (student == null)
                return Json(null);
            return Json(new { firstName = student.FirstName, lastName = student.LastName });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateHearing([FromBody] HearingSchedules model)
        {
            if (model == null || model.Id == 0)
                return Json(new { success = false, error = "Invalid data received." });

            var existing = await _context.HearingSchedules.FindAsync(model.Id);
            if (existing == null)
                return Json(new { success = false, error = "Schedule not found." });

            existing.Title = model.Title ?? existing.Title;
            existing.ScheduledDate = model.ScheduledDate;

            await _context.SaveChangesAsync();

            int idRecord = await _context.HearingSchedules
                                   .Where(r => r.Id == model.Id)
                                   .Select(r => r.StudentNumber)
                                   .FirstOrDefaultAsync();
            
            var studentRecord = await _context.Students.Where(s => s.StudentNumber == idRecord).FirstOrDefaultAsync();

            var emailSubjectVM = new EmailSubjectViewModel
            {
                email = studentRecord!.Email,
                emailMode = 1,
                id = model.Id,
                name = studentRecord.FirstName + " " + studentRecord.LastName,
                hearingSchedule = model.ScheduledDate.ToString("MM/dd/yyyy h:mm tt")
            };

            BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(emailSubjectVM));

            return Json(new { success = true });
        }


        [HttpDelete]
        public async Task<IActionResult> DeleteHearing(int id)
        {
            var existing = await _context.HearingSchedules.FindAsync(id);
            if (existing == null)
                return Json(new { success = false, error = "Schedule not found." });

            _context.HearingSchedules.Remove(existing);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }


        [HttpGet]
        public IActionResult GetHearings()
        {
            var events = _context.HearingSchedules
                .Include(h => h.Student)
                .Select(h => new {
                    id = h.Id,
                    title = h.Title,
                    start = h.ScheduledDate,
                    studentNumber = h.Student.StudentNumber,
                    fullName = h.Student.FirstName + " " + h.Student.LastName,
                    email = h.Student.Email
                })
                .ToList();

            return Json(events);
        }

        [HttpPost]
        public async Task<IActionResult> AddHearing([FromBody] HearingSchedules hearingScheduling)
        {
            if (hearingScheduling == null ||
                string.IsNullOrWhiteSpace(hearingScheduling.Title) ||
                hearingScheduling.ScheduledDate == default ||
                hearingScheduling.StudentNumber == 0)
            {
                return Json(new { success = false, error = "Invalid data received." });
            }

            // Directly use the ScheduledDate from the hearingScheduling object
            DateTime scheduledDate = hearingScheduling.ScheduledDate;

            var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentNumber == hearingScheduling.StudentNumber);
            if (student == null)
            {
                return Json(new { success = false, error = "Student not found." });
            }

            var newSchedule = new HearingSchedules
            {
                Title = hearingScheduling.Title,
                ScheduledDate = scheduledDate, // Store the datetime correctly
                StudentNumber = hearingScheduling.StudentNumber,
                Student = student
            };

            _context.HearingSchedules.Add(newSchedule);
            await _context.SaveChangesAsync();

            int latestRecord = await _context.HearingSchedules
                                   .OrderByDescending(r => r.Id)
                                   .Select(r => r.Id)
                                   .FirstOrDefaultAsync();

            var emailSubjectVM = new EmailSubjectViewModel
            {
                email = student!.Email,
                emailMode = 1,
                id = latestRecord,
                name = student!.FirstName + " " + student!.LastName,
                hearingSchedule = newSchedule.ScheduledDate.ToString("MM/dd/yyyy h:mm tt")
            };

            BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(emailSubjectVM));

            return Json(new
            {
                success = true,
                eventData = new
                {
                    id = newSchedule.Id,
                    title = newSchedule.Title,
                    start = newSchedule.ScheduledDate.ToString("o"), // Ensure the datetime format is consistent
                    studentNumber = student.StudentNumber,
                    fullName = student.FirstName + " " + student.LastName,
                    email = student.Email
                }
            });
        }


    }
}