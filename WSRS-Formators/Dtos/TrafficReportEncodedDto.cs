namespace WSRS_Formators.Dtos;

public class TrafficReportEncodedDto
{
    public OffenseDto Offense { get; set; }
    public DateOnly CommissionDate { get; set; }
    public string Remarks { get; set; }
}
