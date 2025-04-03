using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WSRS_Student.Models
{
    public class TrafficReportsEncoded
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Offense"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int OffenseId { get; set; }
        public Offense Offense { get; set; } // Navigation Property

        [ForeignKey("Student")]
        public int StudentNumber { get; set; }
        [JsonIgnore]
        public Student Student { get; set; } // Navigation Property

        [ForeignKey("College")]
        public string CollegeID { get; set; }
        public College College { get; set; }

        public string PlateNumber { get; set; }
        public DateOnly CommissionDate { get; set; }
        public string Place { get; set; }
        public string Remarks { get; set; }
        public string? ORNumber { get; set; } // NULLABLE
        public DateOnly? DatePaid { get; set; } // NULLABLE
    }
}