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
        public DateOnly CommissionDate { get; set; }
        public DateOnly? HearingDate { get; set; }
        public OffenseClassification Classification { get; set; }
        public string Sanction { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
    }
}