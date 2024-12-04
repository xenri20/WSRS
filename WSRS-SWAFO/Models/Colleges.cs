using System.ComponentModel.DataAnnotations;
using WSRS_SWAFO.Data.Enum;

namespace WSRS_SWAFO.Models
{
    public class Colleges
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
