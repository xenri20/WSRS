namespace WSRS_Api.Dtos;

public class AllReportsDto
{
    public IEnumerable<ReportEncodedDto>? Violations { get; set; }
    public IEnumerable<TrafficReportEncodedDto>? TrafficViolations { get; set; }
    public IEnumerable<ReportsPendingDto>? ReportsPending { get; set; }
}
