using PemitManagement.Data.Enums;

public class PermitListItemViewModel
{
    public int Id { get; set; }

    public string PermitNumber { get; set; } = string.Empty;

    // Combined: Name + (EmpId)
    public string Employee { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public string Agency { get; set; } = string.Empty;

    public DateTime? WorkDate { get; set; }

    public PermitStatus Status { get; set; }

    public int ViolationCount { get; set; }
}
