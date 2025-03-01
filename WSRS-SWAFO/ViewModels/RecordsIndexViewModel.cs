using WSRS_SWAFO.Models;

namespace WSRS_SWAFO.ViewModels;

/// <summary>
/// For searching, filtering and sorting records
/// </summary>
public class RecordsIndexViewModel
{
    public PaginatedList<RecordsViewModel> Pagination { get; set; }
    public string CurrentSort { get; set; }
    public string CommissionDateSort { get; set; }
    public string CurrentFilter { get; set; }
}
