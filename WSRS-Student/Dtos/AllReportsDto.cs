namespace WSRS_Student.Dtos;

public class AllReportsDto
{
    public IEnumerable<ReportEncodedDto>? Violations { get; set; }
    public IEnumerable<TrafficReportEncodedDto>? TrafficViolations { get; set; }
}