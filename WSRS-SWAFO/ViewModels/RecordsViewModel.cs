namespace WSRS_SWAFO.ViewModels
{
    public class RecordsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string OffenseNature { get; set; }
        public int StudentNumber { get; set; }
        public string College { get; set; }
        public string Status { get; set; }
        public DateOnly CommissionDate { get; set; }
    }
}
