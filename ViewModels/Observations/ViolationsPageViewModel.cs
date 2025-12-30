namespace PemitManagement.ViewModels.Observations;

public class ViolationsPageViewModel
{
    public uint PermitId { get; set; }
    public uint PermitTypeId { get; set; }
    public string PermitTypeName { get; set; } = "";

    public string SearchTerm { get; set; } = "";

    public List<string> SelectedCategories { get; set; } = new();
    public List<string> SelectedSeverities { get; set; } = new();

    public List<ViolationListItemVM> AvailableViolations { get; set; } = new();
    public List<SelectedViolationViewModel> SelectedViolations { get; set; } = new();

    public List<string> AllCategories { get; set; } = new();
    public List<string> AllSeverities { get; set; } = new();

    public bool ShowDetailsSection => SelectedViolations.Any();
}
public class ViolationListItemVM
{
    public uint ViolationId { get; set; }
    public string Name { get; set; } = "";
    public string Category { get; set; } = "";
    public string Severity { get; set; } = "";
    public bool IsSelected { get; set; }
}