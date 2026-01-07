namespace PemitManagement.ViewModels.ReportAnalysis
{
    public class ReportAnalysisViewModel
    {
        public ReportAnalysisFilterViewModel Filters { get; set; } = new();

        // Summary buckets (only one is populated at a time)
        public List<EmployeeReportSummaryVM> EmployeeSummary { get; set; } = new();
        public List<AgencyReportSummaryVM> AgencySummary { get; set; } = new();
        public List<RefineryReportSummaryVM> RefinerySummary { get; set; } = new();
        public List<ViolationReportSummaryVM> ViolationSummary { get; set; } = new();
        public List<TimeReportSummaryVM> TimeSummary { get; set; } = new();

        // List section
        public List<ReportViolationRowVM> Rows { get; set; } = new();

        // Pagination
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }

        public int TotalPermits { get; set; }
        public int TotalViolations { get; set; }
        public int PermitsWithViolations { get; set; }

        public List<string> ChartLabels { get; set; } = new();
        public List<int> ChartPermits { get; set; } = new();
        public List<int> ChartViolations { get; set; } = new();

    }

}
