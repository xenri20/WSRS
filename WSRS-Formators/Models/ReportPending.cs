using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WSRS_Formators.Models
{
    public class ReportPending
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("ApplicationUser")]
        public string FormatorId { get; set; }
        //public ApplicationUser Formator { get; set; } // Navigation Property
        
        public string Description { get; set; }
        public DateTime CommissionDatetime { get; set; }

        [ForeignKey("Student")]
        public int StudentNumber { get; set; }
        public Student Student { get; set; } // Navigation Property
    }
}