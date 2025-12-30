namespace PemitManagement.ViewModels.Violations;

public class ViolationListItemViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Category { get; set; } = "";
    public string Severity { get; set; } = "";
    public bool Active { get; set; }
    public List<string> PermitTypes { get; set; } = new();
}
