namespace WSRS_Formators.Models
{
    public class StudentRecordViewModel
    {
        public Student Student { get; set; }
        public List<ReportEncoded> ReportsEncoded { get; set; }
        public List<TrafficReportsEncoded> TrafficReportsEncoded { get; set; }
    }

}
