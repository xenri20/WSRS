using System.ComponentModel.DataAnnotations.Schema;

namespace WSRS_Api.Models;

public class ReportEncoded
{
    public int Id { get; set; }

    [ForeignKey("Offense")]
    public int OffenseId { get; set; }

    public int StudentNumber { get; set; }
    public DateOnly CommissionDate { get; set; }
    public DateOnly? HearingDate { get; set; } // Null if minor
    public string Sanction { get; set; }
    public string Description { get; set; }

    // navigation properties
    public Offense Offense { get; set; }
}
