using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WSRS_SWAFO.Data;
using WSRS_SWAFO.Data.Enum;
using WSRS_SWAFO.Models;
using WSRS_SWAFO.ViewModels;
using WSRS_SWAFO.ViewModels.Records;

namespace WSRS_SWAFO.Controllers
{
    [Authorize(Roles = "AppRole.Admin, AppRole.Member")]
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
            [FromQuery] string? searchString,
            [FromQuery] string? currentSearch,
            [FromQuery] string? college,
            [FromQuery] OffenseClassification? classification,
            [FromQuery] string? statusOfSanction,
            [FromQuery] int? pageIndex)
        {
            var referer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(referer))
            {
                return Content("<script>alert('External links are disabled. Use the in-app interface to proceed.'); window.history.back();</script>", "text/html");
            }

            if (!string.IsNullOrWhiteSpace(searchString))
                pageIndex = 1;
            else
                searchString = currentSearch;

            var query = _context.ReportsEncoded
                        .AsNoTracking()
                        .Include(r => r.Student)
                        .Include(r => r.Offense)
                        .Include(r => r.College)
                        .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim();
                query = query.Where(r => (
                    r.Student.FirstName + " " + r.Student.LastName).Contains(searchString) ||
                    r.StudentNumber.ToString().Contains(searchString)
                );
            }

            // Filters
            if (!string.IsNullOrEmpty(statusOfSanction))
            {
                statusOfSanction = statusOfSanction.Trim();
                query = query.Where(r => r.StatusOfSanction.Equals(statusOfSanction));
            }

            if (!string.IsNullOrEmpty(college))
            {
                query = query.Where(r => r.College.CollegeID.Equals(college));
            }

            if (classification.HasValue)
            {
                query = query.Where(r => r.Offense.Classification == classification.Value);
            }

            switch (sortOrder)
            {
                case "date_asc":
                    query = query.OrderBy(r => r.CommissionDate).ThenBy(r => r.Id);
                    break;
                default:
                    query = query.OrderByDescending(r => r.CommissionDate).ThenBy(r => r.Id);
                    break;
            }

            if (pageIndex < 1) pageIndex = 1;

            int pageSize = 10;

            var recordsAfterFilter = query.Select(r => new RecordsViewModel
            {
                Id = r.Id,
                Name = string.Concat(r.Student.FirstName, " ", r.Student.LastName),
                StudentNumber = r.StudentNumber,
                College = r.CollegeID,
                CommissionDate = r.CommissionDate,
                OffenseClassification = r.Offense.Classification,
                OffenseNature = r.Offense.Nature,
                Status = r.StatusOfSanction,
            });

            var recordsIndexVM = new RecordsIndexViewModel
            {
                Pagination = await PaginatedList<RecordsViewModel>
                    .CreateAsync(recordsAfterFilter, pageIndex ?? 1, pageSize),
                CurrentSort = sortOrder,
                CommissionDateSort = (sortOrder == "date_asc") ? "date_desc" : "date_asc",
                CurrentFilter = new CurrentFilter
                {
                    Search = searchString,
                    College = college,
                    Classification = classification,
                    StatusOfSanction = statusOfSanction,
                },
            };

            // Filter data for dropdowns
            ViewData["CollegeOptions"] = _context.College.Select(c => c.CollegeID).ToList();
            ViewData["ClassificationOptions"] = new List<string>
            {
                OffenseClassification.Minor.ToString(),
                OffenseClassification.Major.ToString()
            };
            ViewData["SanctionStatusOptions"] = new List<string> { "Pending", "Ongoing", "Completed" }; // Might be better if these were enums

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
                Description = record.Description
            };

            return View(recordDetailsVM);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var referer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(referer))
            {
                return Content("<script>alert('External links are disabled. Use the in-app interface to proceed.'); window.history.back();</script>", "text/html");
            }

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
                FirstName = record.Student.FirstName,
                LastName = record.Student.LastName,
                College = record.CollegeID,
                Course = record.Course,
                Nature = record.Offense.Nature,
                Classification = record.Offense.Classification,
                CommissionDate = record.CommissionDate,
                Sanction = record.Sanction,
                StatusOfSanction = record.StatusOfSanction,
                Description = record.Description,
            };

            return View(editRecordVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditRecordViewModel editRecordVM)
        {
            var record = await _context.ReportsEncoded
                .AsNoTracking()
                .Include(r => r.Student)
                .Include(r => r.Offense)
                .FirstOrDefaultAsync(r => r.Id == editRecordVM.Id);

            editRecordVM = new EditRecordViewModel
            {
                Id = record!.Id,
                StudentNumber = record.StudentNumber,
                FirstName = record.Student.FirstName,
                LastName = record.Student.LastName,
                College = record.CollegeID,
                Course = editRecordVM.Course,
                OffenseId = record.OffenseId,
                Classification = record.Offense.Classification,
                Nature = record.Offense.Nature,
                CommissionDate = editRecordVM.CommissionDate,
                Sanction = editRecordVM.Sanction,
                StatusOfSanction = editRecordVM.StatusOfSanction,
                Description = editRecordVM.Description,
            };

            if (!ModelState.IsValid)
            {
                SetToastMessage(message: "Please fill in the required fields appropriately.");
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
                Sanction = editRecordVM.Sanction,
                Description = editRecordVM.Description,
                StatusOfSanction = editRecordVM.StatusOfSanction,
            };

            try
            {
                _context.Update(updatedRecord);
                await _context.SaveChangesAsync();

                SetToastMessage(title: "Success", message: "A record has been edited successfully.");
                return RedirectToAction(nameof(Details), new { id = editRecordVM.Id });
            }
            catch (Exception ex)
            {
                SetToastMessage(title: "Error", message: "Something went wrong modifying your data.", cssClassName: "bg-danger text-white");
                _logger.LogError(ex.Message);
            }

            return View(editRecordVM);
        }

        public async Task<IActionResult> Traffic(
            [FromQuery] string sortOrder,
            [FromQuery] string? searchString,
            [FromQuery] string? currentSearch,
            [FromQuery] string? college,
            [FromQuery] OffenseClassification? classification,
            [FromQuery] bool? settled,
            [FromQuery] int? pageIndex)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                pageIndex = 1;
            else
                searchString = currentSearch;

            var query = _context.TrafficReportsEncoded
                          .AsNoTracking()
                          .Include(tr => tr.Student)
                          .Include(tr => tr.College)
                          .Include(tr => tr.Offense)
                          .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim();
                query = query.Where(tr => (
                    (tr.Student.FirstName + " " + tr.Student.LastName).Contains(searchString) ||
                    tr.StudentNumber.ToString().Contains(searchString)
                ));
            }

            // Filters
            if (settled != null)
            {
                query = (bool)settled
                    ? query.Where(tr => tr.DatePaid != null)
                    : query.Where(tr => tr.DatePaid == null);
            }

            if (!string.IsNullOrEmpty(college))
            {
                query = query.Where(r => r.College.CollegeID.Equals(college));
            }

            if (classification.HasValue)
            {
                query = query.Where(r => r.Offense.Classification == classification.Value);
            }

            switch (sortOrder)
            {
                case "date_asc":
                    query = query.OrderBy(r => r.CommissionDate).ThenBy(r => r.Id);
                    break;
                default:
                    query = query.OrderByDescending(r => r.CommissionDate).ThenBy(r => r.Id);
                    break;
            }

            // Ensures page number is at least 1
            if (pageIndex < 1) pageIndex = 1;

            // Set default page size
            int pageSize = 10;

            var recordsAfterFilter = query.Select(tr => new TrafficRecordsViewModel
            {
                Id = tr.Id,
                Name = string.Concat(tr.Student.FirstName, " ", tr.Student.LastName),
                StudentNumber = tr.StudentNumber,
                College = tr.CollegeID,
                CommissionDate = tr.CommissionDate,
                OffenseClassification = tr.Offense.Classification,
                OffenseNature = tr.Offense.Nature,
                ORNumber = tr.ORNumber,
            });

            var trafficRecordsIndexVM = new TrafficRecordsIndexViewModel
            {
                Pagination = await PaginatedList<TrafficRecordsViewModel>
                    .CreateAsync(recordsAfterFilter, pageIndex ?? 1, pageSize),
                CurrentSort = sortOrder,
                CommissionDateSort = (sortOrder == "date_asc") ? "date_desc" : "date_asc",
                CurrentFilter = new CurrentFilter
                {
                    Search = searchString,
                    College = college,
                    Classification = classification,
                    Settled = settled,
                },
            };

            // Filter data for dropdowns
            ViewData["CollegeOptions"] = _context.College.Select(c => c.CollegeID).ToList();

            return View(trafficRecordsIndexVM);
        }

        public async Task<IActionResult> TrafficDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var record = await _context.TrafficReportsEncoded.AsNoTracking()
                .Include(r => r.Student)
                .Include(r => r.Offense)
                .Include(r => r.College)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (record == null)
            {
                return BadRequest();
            }

            var trafficRecordDetailsVM = new TrafficRecordDetailsViewModel
            {
                Id = record.Id,
                StudentNumber = record.StudentNumber,
                Name = string.Concat(record.Student.FirstName, " ", record.Student.LastName),
                College = record.College.CollegeID,
                OffenseId = record.OffenseId,
                Classification = record.Offense.Classification,
                Nature = record.Offense.Nature,
                CommissionDate = record.CommissionDate,
                PlateNumber = record.PlateNumber,
                Place = record.Place,
                Remarks = record.Remarks,
                DatePaid = record.DatePaid,
                ORNumber = record.ORNumber
            };

            return View(trafficRecordDetailsVM);
        }

        public async Task<IActionResult> EditTraffic(int id)
        {
            var referer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(referer))
            {
                return Content("<script>alert('External links are disabled. Use the in-app interface to proceed.'); window.history.back();</script>", "text/html");
            }

            var record = await _context.TrafficReportsEncoded
                .Include(r => r.Student)
                .Include(r => r.Offense)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (record == null)
            {
                return RedirectToAction(nameof(Traffic));
            }

            var editTrafficRecordVM = new EditTrafficRecordViewModel
            {
                Id = id,
                StudentNumber = record.StudentNumber,
                FirstName = record.Student.FirstName,
                LastName = record.Student.LastName,
                College = record.CollegeID,
                OffenseId = record.OffenseId,
                Classification = record.Offense.Classification,
                Nature = record.Offense.Nature,
                CommissionDate = record.CommissionDate,
                PlateNumber = record.PlateNumber,
                Place = record.Place,
                Remarks = record.Remarks,
                DatePaid = record.DatePaid,
                ORNumber = record.ORNumber,
            };

            return View(editTrafficRecordVM);
        }

        [HttpPost]
        public async Task<IActionResult> EditTraffic(EditTrafficRecordViewModel editTrafficRecordVM)
        {
            var record = await _context.TrafficReportsEncoded
                .AsNoTracking()
                .Include(r => r.Student)
                .Include(r => r.Offense)
                .Include(r => r.College)
                .FirstOrDefaultAsync(r => r.Id == editTrafficRecordVM.Id);

            editTrafficRecordVM = new EditTrafficRecordViewModel
            {
                Id = record!.Id,
                StudentNumber = record.StudentNumber,
                FirstName = record.Student.FirstName,
                LastName = record.Student.LastName,
                College = record.CollegeID,
                OffenseId = record.OffenseId,
                Classification = record.Offense.Classification,
                Nature = record.Offense.Nature,
                CommissionDate = editTrafficRecordVM.CommissionDate,
                PlateNumber = editTrafficRecordVM.PlateNumber,
                Place = editTrafficRecordVM.Place,
                Remarks = editTrafficRecordVM.Remarks,
                DatePaid = editTrafficRecordVM.DatePaid,
                ORNumber = editTrafficRecordVM.ORNumber,
            };

            if (!ModelState.IsValid)
            {
                SetToastMessage(message: "Please fill in the required fields appropriately.");
                return View(editTrafficRecordVM);
            }

            var updatedRecord = new TrafficReportsEncoded
            {
                Id = editTrafficRecordVM.Id,
                StudentNumber = editTrafficRecordVM.StudentNumber,
                CollegeID = editTrafficRecordVM.College,
                OffenseId = editTrafficRecordVM.OffenseId,
                CommissionDate = editTrafficRecordVM.CommissionDate,
                PlateNumber = editTrafficRecordVM.PlateNumber,
                Place = editTrafficRecordVM.Place,
                Remarks = editTrafficRecordVM.Remarks,
                DatePaid = editTrafficRecordVM.DatePaid,
                ORNumber = editTrafficRecordVM.ORNumber,
            };

            try
            {
                _context.Update(updatedRecord);
                await _context.SaveChangesAsync();

                SetToastMessage(title: "Success", message: "A traffic record has been edited successfully.");
                return RedirectToAction(nameof(TrafficDetails), new { id = editTrafficRecordVM.Id });
            }
            catch (Exception ex)
            {
                SetToastMessage(title: "Error", message: "Something went wrong modifying your data.", cssClassName: "bg-danger text-white");
                _logger.LogError(ex.Message);
            }

            return View(editTrafficRecordVM);
        }

        private void SetToastMessage(string message, string title = "", string cssClassName = "bg-white")
        {
            var toastMessage = new ToastViewModel
            {
                Title = title,
                Message = message,
                CssClassName = cssClassName
            };
            TempData["Result"] = JsonSerializer.Serialize(toastMessage);
        }
    }
}
