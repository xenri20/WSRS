using System.ComponentModel.DataAnnotations;
using WSRS_Api.Data.Enum;

namespace WSRS_Api.Models
{
    public class GoodMoralRequest
    {
        [Key]
        public int Id { get; set; }
        public DateOnly ApplicationDate { get; set; }
        public string Purpose { get; set; }
        public string Copies { get; set; }
        public string FullName { get; set; }
        public string Course { get; set; }
        public int StudentNumber { get; set; }
        public Gender Gender { get; set; }
        public string PhoneNumber { get; set; }
        public RequestStatus IsApproved { get; set; }
    }
}
