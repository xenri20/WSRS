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

        public async Task<IActionResult> Index()
        {
            var records = await _context.ReportsEncoded
                .Include(r => r.Student)
                .Select(r => new RecordsViewModel
                {
                    LastName = r.Student.LastName,
                    FirstName = r.Student.FirstName,
                    StudentNumber = r.StudentNumber,
                    College = r.CollegeID,
                    CommissionDate = r.CommissionDate
                })
                .OrderByDescending(r => r.CommissionDate)
                .Skip(0)
                .Take(10)
                .ToListAsync();

            //return Ok(records); // Return data as JSON response
            return View(records.AsQueryable());
        }
    }
}
