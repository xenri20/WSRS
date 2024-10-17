using System.ComponentModel.DataAnnotations;

namespace WSRS_SWAFO.Models
{
    public class LogOn {
        public string? email {  get; set; }
        public string? password { get; set; }
    }
}
