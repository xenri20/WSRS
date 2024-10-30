using System;
using System.ComponentModel.DataAnnotations;
using WSRS_SWAFO.Data.Enum;

namespace WSRS_SWAFO.Models
{
    public class Offense
    {
        [Key]
        public int Id { get; set; }
        public string Nature { get; set; }
        public OffenseClassification Classification { get; set; }
    }
}
