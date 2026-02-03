using WSRS_SWAFO.Models;
using WSRS_SWAFO.ViewModels.Records;

namespace WSRS_SWAFO.ViewModels;
/// <summary>
/// For searching, filtering and sorting traffic records
/// </summary>
public class TrafficRecordsIndexViewModel
{
    public PaginatedList<TrafficRecordsViewModel> Pagination { get; set; }
    public string CurrentSort { get; set; }
    public string CommissionDateSort { get; set; }
    public CurrentFilter CurrentFilter { get; set; }
}
