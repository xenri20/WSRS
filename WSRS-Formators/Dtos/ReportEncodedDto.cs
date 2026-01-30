namespace WSRS_Formators.Dtos;

public class ReportEncodedDto
{
    public OffenseDto Offense { get; set; }
    public DateOnly CommissionDate { get; set; }
    public string Sanction { get; set; }
}
