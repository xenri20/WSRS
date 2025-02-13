using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WSRS_SWAFO.Models
{
    public class Student
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int StudentNumber { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }

        // Navigation Properties
        public ICollection<ReportEncoded> ReportsEncoded { get; set; }
        public ICollection<ReportPending> ReportsPending { get; set; }
    }
}

