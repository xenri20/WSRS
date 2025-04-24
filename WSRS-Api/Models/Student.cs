using System.ComponentModel.DataAnnotations;

namespace WSRS_Api.Models;

public class Student
{
    [Key]
    public int StudentNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
