using System;
using System.Collections.Generic;

namespace PemitManagement.Models;

public partial class PermitResponse
{
    public uint Id { get; set; }

    public uint PermitId { get; set; }

    public string? ResponseText { get; set; }

    public string? Remarks { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Permit Permit { get; set; } = null!;
}
