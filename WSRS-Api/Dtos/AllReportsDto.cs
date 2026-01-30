using WSRS_Api.Models;

namespace WSRS_Api.Dtos;

public class AllReportsDto
{
    public Student Student { get; set; }
    public IEnumerable<ReportEncodedDto>? Violations { get; set; }
    public IEnumerable<TrafficReportEncodedDto>? TrafficViolations { get; set; }
}
