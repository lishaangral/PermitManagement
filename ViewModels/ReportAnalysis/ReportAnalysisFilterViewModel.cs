namespace PemitManagement.ViewModels.ReportAnalysis
{
    using PemitManagement.Data.Enums;

    public class ReportAnalysisFilterViewModel
    {
        public string? PermitNumber { get; set; }
        public string? WorkOrderNumber { get; set; }

        public int? PermitTypeId { get; set; }
        public int? LocationId { get; set; }
        public string? RefineryType { get; set; }
        public int? ViolationId { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public ReportType ReportType { get; set; } = ReportType.Daily;

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
