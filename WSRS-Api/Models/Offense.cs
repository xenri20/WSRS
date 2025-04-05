using WSRS_Api.Data.Enum;

namespace WSRS_Api.Models;

public class Offense
{
    public string Id { get; set; }
    public OffenseClassification OffenseClassification { get; set; }
    public string Nature { get; set; }
}
