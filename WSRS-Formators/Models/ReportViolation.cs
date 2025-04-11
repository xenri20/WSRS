using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WSRS_Formators.Models
{
    public class ReportViolation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int StudentNumber { get; set; }
        public string StudentName { get; set; }
        public string College { get; set; }
        public DateOnly CommissionDate { get; set; }
        public string? Description { get; set; }
    }
}
