using System.ComponentModel.DataAnnotations;

namespace WSRS_Formators.Dtos;

public class ReportPendingDto
{
    [Required]
    public long FormatorId { get; set; }
    [Required]
    public string Formator { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public DateOnly ReportDate { get; set; }
    [Required]
    public int StudentNumber { get; set; }
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    public string College { get; set; }
    [Required]
    public string CourseYearSection { get; set; }
}
