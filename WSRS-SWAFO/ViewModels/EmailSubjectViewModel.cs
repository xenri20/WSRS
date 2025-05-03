using System.ComponentModel.DataAnnotations.Schema;
using WSRS_SWAFO.Models;

namespace WSRS_SWAFO.ViewModels
{
    public class EmailSubjectViewModel
    {
        public string email { get; set; }
        [ForeignKey(nameof(ReportEncoded))]
        public int id { get; set; }
        public string name { get; set; }
        public string sanction { get; set; }
    }
}
