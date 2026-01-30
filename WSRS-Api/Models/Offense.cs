using WSRS_Api.Data.Enum;

namespace WSRS_Api.Models;

public class Offense
{
    public int Id { get; set; }
    public OffenseClassification Classification { get; set; }
    public string Nature { get; set; }
}
