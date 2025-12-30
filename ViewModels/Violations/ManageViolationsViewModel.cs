namespace PemitManagement.ViewModels.Violations;
public class ManageViolationsViewModel
{
    public List<ViolationListItemViewModel> Violations { get; set; } = new();

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalPages { get; set; }

    public string? Search { get; set; }
}
