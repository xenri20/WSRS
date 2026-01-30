using WSRS_Formators.Models;

namespace WSRS_Formators.Dtos
{
    public class AllReportsDto
    {
        public Student? Student { get; set; }
        public IEnumerable<ReportEncodedDto>? Violations { get; set; }
        public IEnumerable<TrafficReportEncodedDto>? TrafficViolations { get; set; }
    }
}
