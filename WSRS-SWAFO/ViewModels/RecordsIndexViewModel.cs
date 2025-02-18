using System;

namespace WSRS_SWAFO.ViewModels;

/// <summary>
/// For searching, filtering and sorting records
/// </summary>
public class RecordsIndexViewModel
{
    public List<RecordsViewModel> Records { get; set; }
    public string CurrentSort { get; set; }
    public string CommissionDateSort { get; set; }
    public string SearchString { get; set; }
}
