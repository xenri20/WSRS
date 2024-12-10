using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WSRS_SWAFO.Models
{
    public class TrafficReportsEncoded
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Offense")]
        public int OffenseId { get; set; }
        public Offense Offense { get; set; } // Navigation Property

        [ForeignKey("Student")]
        public int StudentNumber { get; set; }
        public Student Student { get; set; } // Navigation Property

        [ForeignKey("Colleges")]
        public string CollegeID { get; set; }
        public College College { get; set; }

        public string PlateNumber { get; set; }
        public DateTime CommissionDatetime { get; set; }
        public string Place { get; set; }
        public string Remarks { get; set; }
        public int ORNumber { get; set; }
        public DateOnly DatePaid { get; set; }
    }
}