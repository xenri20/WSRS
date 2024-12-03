using System.ComponentModel.DataAnnotations;

namespace WSRS_SWAFO.Data.Enum
{
    public enum OffenseClassification
    {
        Minor,
        [Display(Name = "Minor Traffic")]
        MinorTraffic,
        Major,
        [Display(Name = "Major Traffic")]
        MajorTraffic
    }
}