using System.ComponentModel.DataAnnotations;

namespace WSRS_Api.Models
{
    public class ReportsPending
    {
        [Key]
        public int Id { get; set; }
        public long FormatorId { get; set; }
        public string Formator { get; set; }
        public string Description { get; set; }
        public DateOnly ReportDate { get; set; }
        public int StudentNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string College { get; set; }
        public string CourseYearSection { get; set; }
        public bool IsArchived { get; set; }
    }
}
