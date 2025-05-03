using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WSRS_Formators.Models
{
    [Index(nameof(StudentNumber))]
    public class Student
    {
        [Key]
        public int StudentNumber { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
    }
}

