using System.ComponentModel.DataAnnotations;

namespace WSRS_Api.Data.Enum;

public enum OffenseClassification
{
    Minor,
    Major,
    [Display(Name = "Minor Traffic")]
    MinorTraffic,
    [Display(Name = "Major Traffic")]
    MajorTraffic
}
