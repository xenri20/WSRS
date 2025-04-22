using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WSRS_Api.Dtos
{
    public class ReportsPendingDto
    {
        [Required]
        public int FormatorId { get; set; }
        [Required]
        public string Formator { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public DateOnly ReportDate { get; set; }
        [Required]
        public int StudentNumber { get; set; }
        [Required]
        public bool IsArchived { get; set; }
    }
}
