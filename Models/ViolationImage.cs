using System;
using System.Collections.Generic;

namespace PemitManagement.Models;

public partial class ViolationImage
{
    public uint Id { get; set; }

    public uint PermitViolationId { get; set; }

    public string ImagePath { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual PermitViolation PermitViolation { get; set; } = null!;
}
