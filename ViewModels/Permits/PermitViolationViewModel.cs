using PemitManagement.Data.Enums;

public class PermitViolationViewModel
{
    public int PermitViolationId { get; set; }
    public string ViolationName { get; set; } = "";
    public string Category { get; set; } = "";
    public string Severity { get; set; } = "";
    public string Remarks { get; set; } = "";
    public string ActionTaken { get; set; } = "";
    public string CreatedBy { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public ObservationStatus Status { get; set; }

    public List<string> Images { get; set; } = new();
    public bool IsPermitClosed { get; set; }

}
