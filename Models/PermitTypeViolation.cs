using System;
using System.Collections.Generic;

namespace PemitManagement.Models;

public partial class PermitTypeViolation
{
    public uint Id { get; set; }

    public uint PermitTypeId { get; set; }

    public uint ViolationId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual PermitType PermitType { get; set; } = null!;

    public virtual Violation Violation { get; set; } = null!;
}
