using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using WSRS_SWAFO.Data;

namespace WSRS_SWAFO.Controllers
{
    public class HearingSchedulingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HearingSchedulingController> _logger;

        public HearingSchedulingController(ApplicationDbContext context, ILogger<HearingSchedulingController> logger)
        {
            _context = context;
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

            var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentNumber == hearingScheduling.StudentNumber);
            if (student == null)
            {
                return Json(new { success = false, error = "Student not found." });
            }

            var newSchedule = new HearingSchedules
            {
                Title = hearingScheduling.Title,
                ScheduledDate = hearingScheduling.ScheduledDate,
                StudentNumber = hearingScheduling.StudentNumber,
                Student = student
            };

            _context.HearingSchedules.Add(newSchedule);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                eventData = new
                {
                    id = newSchedule.Id,
                    title = newSchedule.Title,
                    start = newSchedule.ScheduledDate.ToString("o"),
                    studentNumber = student.StudentNumber,
                    fullName = student.FirstName + " " + student.LastName,
                    email = student.Email
                }
            });
        }





    }
}