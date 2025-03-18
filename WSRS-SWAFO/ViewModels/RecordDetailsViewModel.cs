using WSRS_SWAFO.Data.Enum;

namespace WSRS_SWAFO.ViewModels
{
    public class RecordDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int StudentNumber { get; set; }
        public string Course { get; set; }
        public string College { get; set; }
        public string? Formator { get; set; }
        public int OffenseId { get; set; }
        public string Nature { get; set; }
        public OffenseClassification Classification { get; set; }
        public DateOnly CommissionDate { get; set; }
        public string FormattedCommissionDate => CommissionDate.ToString("MM/dd/yyyy");
        public DateOnly? HearingDate { get; set; }
        public string FormattedHearingDate => HearingDate.HasValue ? HearingDate.Value.ToString("MM/dd/yyyy") : string.Empty;
        public string Sanction { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
    }
}