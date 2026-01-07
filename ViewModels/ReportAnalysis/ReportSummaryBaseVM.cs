namespace PemitManagement.ViewModels.ReportAnalysis
{
    public abstract class ReportSummaryBaseVM
    {
        public int TotalPermits { get; set; }
        public int TotalViolations { get; set; }
        public int PermitsWithViolations { get; set; }
    }

    public class EmployeeReportSummaryVM : ReportSummaryBaseVM
    {
        public string EmployeeName { get; set; } = "";
        public DateTime FirstPermit { get; set; }
        public DateTime LastPermit { get; set; }
        public int EntryDays { get; set; }
        public int ActionTaken { get; internal set; }
    }

    public class AgencyReportSummaryVM : ReportSummaryBaseVM
    {
        public string AgencyName { get; set; } = "";
        public int WorkOrders { get; set; }
    }

    public class RefineryReportSummaryVM : ReportSummaryBaseVM
    {
        public string RefineryType { get; set; } = "";
    }

    public class ViolationReportSummaryVM : ReportSummaryBaseVM
    {
        public string ViolationName { get; set; } = "";
        public string Severity { get; set; } = "";
    }

    public class TimeReportSummaryVM : ReportSummaryBaseVM
    {
        public string PeriodLabel { get; set; } = ""; 
    }
}
