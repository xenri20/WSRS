using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WSRS_Formators.Models
{
    [Index(nameof(StudentNumber))]
    public class Student
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int StudentNumber { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string IdentityUserId { get; set; }

        // Navigation Properties
        public ICollection<ReportEncoded> ReportsEncoded { get; set; }
        public ICollection<TrafficReportsEncoded> TrafficReportsEncoded { get; set; }
        public ICollection<ReportPending> ReportsPending { get; set; }
    }
}

