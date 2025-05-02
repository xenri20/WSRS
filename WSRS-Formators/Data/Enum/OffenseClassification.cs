using System.ComponentModel.DataAnnotations;

namespace WSRS_Formators.Data.Enum
{
    public enum OffenseClassification
    {
        Minor,
        Major,
        [Display(Name ="Minor Traffic")]
        MinorTraffic,
        [Display(Name = "Major Traffic")]
        MajorTraffic
    }
}
