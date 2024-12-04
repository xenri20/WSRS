using System;
using System.ComponentModel.DataAnnotations;
using WSRS_SWAFO.Data.Enum;

namespace WSRS_SWAFO.Models
{
    public class Offense
    {
        [Key]
        public int Id { get; set; }
        public OffenseClassification Nature { get; set; }
        public string Classification { get; set; }
    }
}
