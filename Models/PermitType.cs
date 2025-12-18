using System;
using System.Collections.Generic;

namespace PemitManagement.Models;

public partial class PermitType
{
    public uint Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool? Active { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<PermitTypeViolation> PermitTypeViolations { get; set; } = new List<PermitTypeViolation>();

    public virtual ICollection<Permit> Permits { get; set; } = new List<Permit>();
}
