namespace WSRS_Api.Dtos;

public class ReportEncodedDto
{
    public OffenseDto Offense { get; set; }
    public string Description { get; set; }
    public DateOnly CommissionDate { get; set; }
    public string Sanction { get; set; }
}
