using Microsoft.AspNetCore.Mvc.Rendering;
using WSRS_Student.Data.Enum;

namespace WSRS_Student.ViewModels
{
    public class GoodMoralRequestViewModel
    {
        public DateOnly ApplicationDate { get; set; }
        public string Purpose { get; set; }
        public string Copies { get; set; }
        public string FullName { get; set; }
        public string Course { get; set; }
        public int StudentNumber { get; set; }
        public Gender Gender { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsApproved { get; set; }
    }
}
