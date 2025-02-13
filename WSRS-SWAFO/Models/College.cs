using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WSRS_SWAFO.Data.Enum;

namespace WSRS_SWAFO.Models
{
    public class College
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string CollegeID { get; set; }
        
    }
}
