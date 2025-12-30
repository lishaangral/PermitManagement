using PemitManagement.Data.Enums;

namespace PemitManagement.ViewModels.Permits;

public class PermitExportRow
{
    public string PermitNumber { get; set; } = "";
    public string Employee { get; set; } = "";
    public string PermitType { get; set; } = "";
    public string Location { get; set; } = "";
    public string Agency { get; set; } = "";
    public string WorkDate { get; set; } = "";
    public PermitStatus Status { get; set; }
    public int ViolationCount { get; set; }
}
