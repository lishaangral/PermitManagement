using PemitManagement.Data.Enums;

namespace PemitManagement.ViewModels.ReportAnalysis
{
    public class ReportViolationRowVM
    {
        public int PermitId { get; set; }
        public string PermitNumber { get; set; } = "";
        public string Employee { get; set; } = "";
        public string Agency { get; set; } = "";
        public string WorkOrder { get; set; } = "";

        public string Violation { get; set; } = "";
        public string Category { get; set; } = "";
        public string Severity { get; set; } = "";

        public ObservationStatus ObservationStatus { get; set; }
        public PermitStatus PermitStatus { get; set; }

        public DateTime CreatedAt { get; set; }
    }

}
