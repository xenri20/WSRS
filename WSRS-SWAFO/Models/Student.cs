using System;
using System.ComponentModel.DataAnnotations;

namespace WSRS_SWAFO.Models
{
    public class Student
    {
        [Key]
        public int StudentNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Course { get; set; }

        // Navigation Properties
        public ICollection<ReportEncoded> ReportsEncoded { get; set; }
        public ICollection<ReportPending> ReportsPending { get; set; }
    }
}

