using System.ComponentModel.DataAnnotations;

namespace WSRS_SWAFO.Models
{
    public class ReportPending
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
        public bool IsArchived { get; set; }
    }
}