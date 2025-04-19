using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WSRS_SWAFO.Models
{
    public class ReportPending
    {
        [Key]
        public int Id { get; set; }

        // Should be changed later since database is different
        [ForeignKey("ApplicationUser")]
        public string FormatorId { get; set; }
        public ApplicationUser Formator { get; set; } // Navigation Property
        
        public string Description { get; set; }
        public DateOnly ReportDate { get; set; }

        [ForeignKey("Student")]
        public int StudentNumber { get; set; }
        public Student Student { get; set; } // Navigation Property
    }
}