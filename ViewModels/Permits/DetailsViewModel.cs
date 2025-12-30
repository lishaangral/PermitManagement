using PemitManagement.Data.Enums;
using PemitManagement.ViewModels.Observations;

public class DetailsViewModel
{
    // Permit core
    public int PermitId { get; set; }
    public string PermitNumber { get; set; } = "";
    public string PermitType { get; set; } = "";
    public string StatusText => Status.ToString();
    public PermitStatus Status { get; set; }

    // Dates
    public DateTime? WorkDate { get; set; }
    public DateTime? LatestObservationAt { get; set; }

    // Organization
    public string AgencyName { get; set; } = "";
    public string WorkOrderNumber { get; set; } = "";
    public string ContractWorkerName { get; set; } = "";

    // Location
    public string Location { get; set; } = "";
    public string Refinery { get; set; } = "";
    public string ExactLocation { get; set; } = "";

    // People
    public string EmployeeName { get; set; } = "";
    public string EmployeeId { get; set; } = "";
    public string ReceiverName { get; set; } = "";
    public string ReceiverEmployeeId { get; set; } = "";

    // Issuers
    public List<IssuerInfoViewModel> Issuers { get; set; } = new();

    // Violations
    public List<PermitViolationViewModel> OpenViolations { get; set; } = new();
    public List<PermitViolationViewModel> ClosedViolations { get; set; } = new();
}
