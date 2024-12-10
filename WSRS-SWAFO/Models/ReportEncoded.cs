using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WSRS_SWAFO.Models
{
    public class ReportEncoded
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Offense")]
        public int OffenseId { get; set; }
        public Offense Offense { get; set; } // Navigation Property

        [ForeignKey("Colleges")]
        public string CollegeID { get; set; }
        public Colleges Colleges { get; set; }

        [ForeignKey("Student")]
        public int StudentNumber { get; set; }
        public Student Student { get; set; } // Navigation Property

        [ForeignKey("ApplicationUser")]
        public string? FormatorId { get; set; } // Nullable if needed
        public ApplicationUser Formator { get; set; }

        public DateOnly CommissionDate { get; set; }
        public string Course { get; set; }
        public DateOnly HearingDate { get; set; }
        public string Sanction { get; set; }
        public string Description { get; set; }
        public string StatusOfSanction { get; set; }
    }
}
