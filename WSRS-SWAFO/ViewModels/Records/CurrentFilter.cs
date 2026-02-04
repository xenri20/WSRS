using WSRS_SWAFO.Data.Enum;

namespace WSRS_SWAFO.ViewModels.Records
{
    public class CurrentFilter
    {
        public string? Search { get; set; }
        public string? College { get; set; }
        public OffenseClassification? Classification { get; set; }
        public string? StatusOfSanction { get; set; }
        public bool? Settled { get; set; }
    }
}
