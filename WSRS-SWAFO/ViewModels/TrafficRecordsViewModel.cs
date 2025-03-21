namespace WSRS_SWAFO.ViewModels
{
    public class TrafficRecordsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string OffenseClassification { get; set; }
        public string OffenseNature { get; set; }
        public int StudentNumber { get; set; }
        public string College { get; set; }
        public DateOnly CommissionDate { get; set; }
        public string FormattedCommissionDate => CommissionDate.ToString("MM/dd/yyyy");

        public string PlateNumber { get; set; }
        public string Remarks { get; set; }
        public string? ORNumber { get; set; } // NULLABLE
        public DateOnly? DatePaid { get; set; } // NULLABLE
    }
}
