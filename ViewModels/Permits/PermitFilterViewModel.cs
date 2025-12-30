using PemitManagement.Data.Enums;

public class PermitFilterViewModel
{
    public string? PermitNumber { get; set; }

    public string? EmployeeId { get; set; }

    public int? PermitTypeId { get; set; }

    public int? LocationId { get; set; }

    public PermitStatus? Status { get; set; }

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public string? DatePreset { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public string? ViolationFilter { get; set; }

}
