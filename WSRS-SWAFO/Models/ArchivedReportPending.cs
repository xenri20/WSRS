using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WSRS_SWAFO.Models
{
    [Index(nameof(ReportPendingId), IsUnique = true)]
    public class ArchivedReportPending
    {
        [Key]
        public int Id { get; set; } 

        [ForeignKey(nameof(ReportPending))]
        public int ReportPendingId { get; set; }

        public DateTime ArchivedAt { get; set; }
    }
}
