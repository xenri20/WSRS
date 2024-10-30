using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WSRS_SWAFO.Models
{
    public class ReportEncoded
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Offense")]
        public int OffenseId { get; set; }
        [ForeignKey("Student")]
        public int StudentNumber { get; set; }
        public DateTime CommissionDatetime { get; set; }
        public string Sanction { get; set; }
        public string Description { get; set; }
    }
}