using System;
using System.Collections.Generic;

namespace PemitManagement.Models;

public partial class PermitViolation
{
    public uint Id { get; set; }

    public uint PermitId { get; set; }

    public uint ViolationId { get; set; }

    public string? Remarks { get; set; }

    public string? ActionTaken { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Permit Permit { get; set; } = null!;

    public virtual Violation Violation { get; set; } = null!;

    public virtual ICollection<ViolationImage> ViolationImages { get; set; } = new List<ViolationImage>();
}
