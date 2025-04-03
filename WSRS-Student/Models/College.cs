using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WSRS_Student.Models
{
    public class College
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string CollegeID { get; set; }
        
    }
}
