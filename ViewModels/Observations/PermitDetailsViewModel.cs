using System.ComponentModel.DataAnnotations;

namespace PemitManagement.ViewModels.Observations
{
    public class PermitDetailsViewModel
    {
        public uint PermitId { get; set; }

        [Required(ErrorMessage = "Permit number is required")]
        public string? PermitNumber { get; set; }

        public bool HasSearched { get; set; }
        public bool PermitFound { get; set; }

        // Permit Information
        public string? PermitType { get; set; }
        public uint PermitTypeId { get; set; }
        public string? AgencyName { get; set; }
        public string? WorkOrderNumber { get; set; }
        public string? ContractWorkerName { get; set; }

        // Location Information
        public string? Refinery { get; set; }
        public string? VisitedSite { get; set; }
        public string? ExactLocation { get; set; }

        // Issuers
        public List<IssuerInfoViewModel> Issuers { get; set; } = new();

        // Receiver
        public string? ReceiverName { get; set; }
        public string? ReceiverEmployeeId { get; set; }
    }

    public class IssuerInfoViewModel
    {
        public string RoleName { get; set; } = "";
        public string EmployeeId { get; set; } = "";
        public string EmployeeName { get; set; } = "";
    }
}
