using System.ComponentModel.DataAnnotations;
using WSRS_SWAFO.Data.Enum;
using WSRS_SWAFO.Dtos;

namespace WSRS_SWAFO.ViewModels
{
    public class GoodMoralRequestViewModel
    {
        public IEnumerable<GMCRequestDto> GMCRequests { get; set; }
    }
}
