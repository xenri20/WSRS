using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WSRS_SWAFO.Models
{
    public class ReportEncoded
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Offense)), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int OffenseId { get; set; }
        public Offense Offense { get; set; } // Navigation Property

        [ForeignKey(nameof(College))]
        public string CollegeID { get; set; }
        public College College { get; set; }

        [ForeignKey(nameof(Student))]
        public int StudentNumber { get; set; }
        [JsonIgnore]
        public Student Student { get; set; } // Navigation Property

        public string? FormatorId { get; set; } // Nullable if needed
        public string? Formator { get; set; }

        public DateOnly CommissionDate { get; set; }
        public string Course { get; set; }
        public string Sanction { get; set; }
        public string Description { get; set; }
        public string StatusOfSanction { get; set; }
    }
}
