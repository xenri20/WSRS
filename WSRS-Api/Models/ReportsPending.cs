using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WSRS_Api.Models;

namespace WSRS_Api.Models
{
    public class ReportsPending
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