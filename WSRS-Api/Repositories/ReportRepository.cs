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

    }
}
