using Microsoft.AspNetCore.Mvc.Rendering;

public class PermitIndexViewModel
{
    public List<PermitListItemViewModel> Permits { get; set; } = new();

    public PermitFilterViewModel Filters { get; set; } = new();

    // Lookups for dropdowns
    public List<SelectListItem> Employees { get; set; } = new();

    public List<SelectListItem> PermitTypes { get; set; } = new();

    public List<SelectListItem> Locations { get; set; } = new();

    public int TotalCount { get; set; }

    public int TotalPages { get; set; }
}
