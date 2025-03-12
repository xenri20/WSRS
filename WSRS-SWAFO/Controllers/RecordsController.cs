using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WSRS_SWAFO.Data;
using WSRS_SWAFO.Models;
using WSRS_SWAFO.ViewModels;

namespace WSRS_SWAFO.Controllers
{
    public class RecordsController : Controller
    {
        private readonly ILogger<ReportsController> _logger;
        private readonly ApplicationDbContext _context;

        public RecordsController(ApplicationDbContext context, ILogger<ReportsController> logger)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index(
            [FromQuery] string sortOrder,
            [FromQuery] string searchString,
            [FromQuery] string currentFilter,
            [FromQuery] int? pageIndex)
        {
            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            var records = from r in _context.ReportsEncoded.AsNoTracking()
                .Include(r => r.Student)
                          select new RecordsViewModel
                          {
                              Id = r.Id,
                              Name = string.Concat(r.Student.FirstName, " ", r.Student.LastName),
                              StudentNumber = r.StudentNumber,
                              College = r.CollegeID,
                              CommissionDate = r.CommissionDate,
                              OffenseClassification = r.Offense.Classification.ToString(),
                              OffenseNature = r.Offense.Nature,
                              Status = r.StatusOfSanction
                          };

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim();
                records = records.Where(r => r.Name.Contains(searchString)
                                            || r.StudentNumber.ToString().Contains(searchString));
            }

            switch (sortOrder)
            {
                case "date_asc":
                    records = records.OrderBy(r => r.CommissionDate).ThenBy(r => r.Id);
                    break;
                default:
                    records = records.OrderByDescending(r => r.CommissionDate).ThenBy(r => r.Id);
                    break;
            }

            // Ensures page number is at least 1
            if (pageIndex < 1)
            {
                pageIndex = 1;
            }

            // Set default page size
            int pageSize = 10;

            var recordsIndexVM = new RecordsIndexViewModel
            {
                Pagination = await PaginatedList<RecordsViewModel>.CreateAsync(records, pageIndex ?? 1, pageSize),
                CurrentSort = sortOrder,
                CommissionDateSort = (sortOrder == "date_asc") ? "date_desc" : "date_asc",
                CurrentFilter = searchString,
            };

            //return Ok(records); // Return data as JSON response
            return View(recordsIndexVM);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var record = await _context.ReportsEncoded.AsNoTracking()
                .Include(r => r.Student)
                .Include(r => r.Offense)
                .Include(r => r.College)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (record == null)
            {
                return BadRequest();
            }

            var recordDetailsVM = new RecordDetailsViewModel
            {
                Id = record.Id,
                StudentNumber = record.StudentNumber,
                Name = string.Concat(record.Student.FirstName, " ", record.Student.LastName),
                Course = record.Course,
                College = record.College.CollegeID,
                CommissionDate = record.CommissionDate,
                OffenseId = record.OffenseId,
                Nature = record.Offense.Nature,
                Classification = record.Offense.Classification,
                Sanction = record.Sanction,
                Status = record.StatusOfSanction,
                HearingDate = record.HearingDate,
                Description = record.Description
            };

            return View(recordDetailsVM);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var record = await _context.ReportsEncoded
                .Include(r => r.Student)
                .Include(r => r.Offense)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (record == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var editRecordVM = new EditRecordViewModel
            {
                Id = id,
                StudentNumber = record.StudentNumber,
                Student = record.Student,
                College = record.CollegeID,
                Course = record.Course,
                OffenseId = record.OffenseId,
                Offense = record.Offense,
                CommissionDate = record.CommissionDate,
                HearingDate = record.HearingDate,
                Sanction = record.Sanction,
                StatusOfSanction = record.StatusOfSanction,
                Description = record.Description,
            };

            return View(editRecordVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditRecordViewModel editRecordVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit record.");
                return View(editRecordVM);
            }

            var updatedRecord = new ReportEncoded
            {
                Id = editRecordVM.Id,
                StudentNumber = editRecordVM.StudentNumber,
                CollegeID = editRecordVM.College,
                OffenseId = editRecordVM.OffenseId,
                Course = editRecordVM.Course,
                CommissionDate = editRecordVM.CommissionDate,
                HearingDate = editRecordVM.HearingDate,
                Sanction = editRecordVM.Sanction,
                Description = editRecordVM.Description,
                StatusOfSanction = editRecordVM.StatusOfSanction,
            };

            _context.Update(updatedRecord);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = editRecordVM.Id });
        }

        public async Task<IActionResult> Traffic(
            [FromQuery] string sortOrder,
            [FromQuery] string searchString,
            [FromQuery] string currentFilter,
            [FromQuery] int? pageIndex)
        {
            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            var records = from tr in _context.TrafficReportsEncoded.AsNoTracking()
                    .Include(tr => tr.Student)
                          select new TrafficRecordsViewModel
                          {
                              Id = tr.Id,
                              Name = string.Concat(tr.Student.FirstName, " ", tr.Student.LastName),
                              StudentNumber = tr.StudentNumber,
                              College = tr.CollegeID,
                              CommissionDate = tr.CommissionDate,
                              OffenseClassification = tr.Offense.Classification.ToString().Substring(0, 5) + " Traffic",
                              OffenseNature = tr.Offense.Nature,
                              DatePaid = tr.DatePaid,
                          };

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim();
                records = records.Where(r => r.Name.Contains(searchString)
                                            || r.StudentNumber.ToString().Contains(searchString));
            }

            switch (sortOrder)
            {
                case "date_asc":
                    records = records.OrderBy(r => r.CommissionDate).ThenBy(r => r.Id);
                    break;
                default:
                    records = records.OrderByDescending(r => r.CommissionDate).ThenBy(r => r.Id);
                    break;
            }

            // Ensures page number is at least 1
            if (pageIndex < 1)
            {
                pageIndex = 1;
            }

            // Set default page size
            int pageSize = 10;

            var recordsIndexVM = new TrafficRecordsIndexViewModel
            {
                Pagination = await PaginatedList<TrafficRecordsViewModel>.CreateAsync(records, pageIndex ?? 1, pageSize),
                CurrentSort = sortOrder,
                CommissionDateSort = (sortOrder == "date_asc") ? "date_desc" : "date_asc",
                CurrentFilter = searchString,
            };

            //return Ok(records); // Return data as JSON response
            return View(recordsIndexVM);
        }
    }
}
