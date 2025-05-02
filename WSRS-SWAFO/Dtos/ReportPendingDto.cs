using System.ComponentModel.DataAnnotations;

namespace WSRS_SWAFO.Dtos
{
    public class ReportPendingDto
    {
        public int Id { get; set; }
        public long FormatorId { get; set; }
        public string Formator { get; set; }
        public string Description { get; set; }

        [Display(Name = "Report Date")]
        public DateOnly ReportDate { get; set; }

        [Display(Name = "Student Number")]
        public int StudentNumber { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public string College { get; set; }

        [Display(Name = "CYS")]
        public string CourseYearSection { get; set; }

        public bool IsArchived { get; set; }
        public string FormattedReportDate => ReportDate.ToString("MM/dd/yyyy");
    }
}
