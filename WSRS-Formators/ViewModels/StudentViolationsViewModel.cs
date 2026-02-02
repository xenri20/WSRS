using WSRS_Formators.Dtos;
using WSRS_Formators.Models;

namespace WSRS_Formators.ViewModels;

public class StudentViolationsViewModel
{
    public int? StudentNumber { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FullName => FirstName + " " + LastName;
    public IEnumerable<ReportEncodedDto>? Violations { get; set; }
    public IEnumerable<TrafficReportEncodedDto>? TrafficViolations { get; set; }
}
