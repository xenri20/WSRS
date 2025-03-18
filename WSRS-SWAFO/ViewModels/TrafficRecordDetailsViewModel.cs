using WSRS_SWAFO.Data.Enum;

namespace WSRS_SWAFO.ViewModels
{
    public class TrafficRecordDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int StudentNumber { get; set; }
        public string College { get; set; }
        public int OffenseId { get; set; }
        public string Nature { get; set; }
        public OffenseClassification Classification { get; set; }
        public DateOnly CommissionDate { get; set; }
        public string FormattedCommissionDate => CommissionDate.ToString("MM/dd/yyyy");
        public string PlateNumber { get; set; }
        public string Place { get; set; }
        public string Remarks { get; set; }
        public DateOnly? DatePaid { get; set; }
        public string? FormattedDatePaid => DatePaid.HasValue ? DatePaid.Value.ToString("MM/dd/yyyy") : string.Empty;
        public int? ORNumber { get; set; }
    }
}
