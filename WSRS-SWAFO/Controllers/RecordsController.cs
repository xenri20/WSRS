using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WSRS_SWAFO.Data;
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
            [FromQuery] string searchString)
        {
            var records = from r in _context.ReportsEncoded
                .Include(r => r.Student)
                          select new RecordsViewModel
                          {
                              Id = r.Id,
                              Name = string.Concat(r.Student.FirstName, " ", r.Student.LastName),
                              StudentNumber = r.StudentNumber,
                              College = r.CollegeID,
                              CommissionDate = r.CommissionDate,
                              OffenseNature = r.Offense.Nature
                          };

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim();
                records = records.Where(r => r.Name.Contains(searchString)
                                            || r.StudentNumber.ToString().Contains(searchString));
            }

            switch (sortOrder)
            {
                case "date_desc":
                    records = records.OrderByDescending(r => r.CommissionDate).ThenBy(r => r.Id);
                    break;
                default:
                    records = records.OrderBy(r => r.CommissionDate).ThenBy(r => r.Id);
                    break;
            }

            var recordsIndexVM = new RecordsIndexViewModel
            {
                Records = await records.ToListAsync(),
                CurrentSort = sortOrder,
                CommissionDateSort = (sortOrder == "date_desc") ? "date_asc" : "date_desc",
                SearchString = searchString,
            };

            //return Ok(records); // Return data as JSON response
            return View(recordsIndexVM);
        }
    }
}
