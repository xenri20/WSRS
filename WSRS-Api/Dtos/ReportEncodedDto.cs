namespace WSRS_Api.Dtos;

public class ReportEncodedDto
{
    public int OffenseId { get; set; }
    public DateOnly CommissionDate { get; set; }
    public string? Description { get; set; }
    public string Sanction { get; set; }
}
