using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WSRS_Api.Models;

public class TrafficReportEncoded
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Offense"), DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int OffenseId { get; set; }

    [ForeignKey("Student")]
    public int StudentNumber { get; set; }
    public string PlateNumber { get; set; }
    public DateOnly CommissionDate { get; set; }
    public string Place { get; set; }
    public string Remarks { get; set; }
    public string? ORNumber { get; set; } // NULLABLE
    public DateOnly? DatePaid { get; set; } // NULLABLE

    // navigation properties
    public Offense Offense { get; set; }
}
