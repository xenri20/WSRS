using System.ComponentModel.DataAnnotations.Schema;

namespace WSRS_Api.Dtos
{
    public class ReportPendingDto
    {
        public string FormatorId { get; set; } 
        public string Description { get; set; }
        public DateTime CommissionDatetime { get; set; }
        public int StudentNumber { get; set; }
    }
}
