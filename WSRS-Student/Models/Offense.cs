using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WSRS_Student.Data.Enum;

namespace WSRS_Student.Models
{
    public class Offense
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public OffenseClassification Classification { get; set; }
        public string Nature { get; set; }
    }
}
