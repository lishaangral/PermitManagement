using PemitManagement.Data.Enums;
using System;
using System.Collections.Generic;
namespace PemitManagement.Models;

public partial class Permit
{
    public uint Id { get; set; }

    public string EmployeeId { get; set; } = null!;

    public string EmployeeName { get; set; } = null!;

    public string? ContractWorkerName { get; set; }

    public string? AgencyName { get; set; }

    public string? WorkOrderNumber { get; set; }

    public string PermitNumber { get; set; } = null!;

    public string? IssuerName { get; set; }

    public string? IssuerEmployeeId { get; set; }

    public string? ReceiverName { get; set; }

    public string? ReceiverEmployeeId { get; set; }

    public uint LocationId { get; set; }

    public uint PermitTypeId { get; set; }

    public DateOnly? WorkDate { get; set; }

    public TimeOnly? WorkTime { get; set; }

    public int IncidentCount { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string? ExactLocation { get; set; }

    public virtual Location Location { get; set; } = null!;

    public virtual ICollection<PermitResponse> PermitResponses { get; set; } = new List<PermitResponse>();

    public virtual PermitType PermitType { get; set; } = null!;

    public virtual ICollection<PermitViolation> PermitViolations { get; set; } = new List<PermitViolation>();

    public PermitStatus Status { get; set; } = PermitStatus.Open;

    public DateTime? ClosedAt { get; set; }

    public string? ClosedBy { get; set; }
}
