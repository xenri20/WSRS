using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WSRS_Api.Models;

namespace WSRS_Api.Models
{
    public class ReportPending
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("ApplicationUser")]
        public string FormatorId { get; set; } // Navigation Property

        public string Description { get; set; }
        public DateTime CommissionDatetime { get; set; }

        [ForeignKey("Student")]
        public int StudentNumber { get; set; }
        public Student Student { get; set; } // Navigation Property
    }
}