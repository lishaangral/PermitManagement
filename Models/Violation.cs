using System;
using System.Collections.Generic;

namespace PemitManagement.Models;

public partial class Violation
{
    public uint Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Category { get; set; }

    public string? Severity { get; set; }

    public bool? Active { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<PermitTypeViolation> PermitTypeViolations { get; set; } = new List<PermitTypeViolation>();

    public virtual ICollection<PermitViolation> PermitViolations { get; set; } = new List<PermitViolation>();
}
