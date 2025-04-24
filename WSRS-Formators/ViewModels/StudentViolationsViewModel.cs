using WSRS_Formators.Dtos;
using WSRS_Formators.Models;

namespace WSRS_Formators.ViewModels;

public class StudentViolationsViewModel
{
    public Student Student { get; set; }
    public IEnumerable<ReportEncodedDto>? Violations { get; set; }
    public IEnumerable<TrafficReportEncodedDto>? TrafficViolations { get; set; }
}