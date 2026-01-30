using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WSRS_SWAFO.Data;
using WSRS_SWAFO.Models;

namespace WSRS_SWAFO.Controllers
{
    [Authorize(Roles = "AppRole.Admin")]
    public class OffensesController : Controller
    {
        private readonly ILogger<OffensesController> _logger;
        private readonly ApplicationDbContext _context;

        public OffensesController(ApplicationDbContext context, ILogger<OffensesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            throw new NotImplementedException();
        }

        [Route("api/[controller]/GetAllDescriptions")]
        [HttpGet]
        [Produces("application/json")]
        public async Task<ActionResult<ReportEncoded>> GetAllPreviousDescriptions()
        {
            var reports = await _context.ReportsEncoded
                .Select(r => r.Description)
                .Distinct()
                .ToListAsync();

            return Ok(reports);
        }
    }
}
