using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WSRS_SWAFO.Models
{
    public class ReportPending
    {
        [Key]
        public int Id { get; set; }
        public int FormatorId { get; set; }
        public string Formator { get; set; } 
        public string Description { get; set; }
        public DateOnly ReportDate { get; set; }
        public int StudentNumber { get; set; }
        public bool IsArchived { get; set; }
    }
}