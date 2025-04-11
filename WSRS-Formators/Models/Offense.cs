using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WSRS_Formators.Data.Enum;

namespace WSRS_Formators.Models
{
    public class Offense
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public OffenseClassification Classification { get; set; }
        public string Nature { get; set; }
    }
}
