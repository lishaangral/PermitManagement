using System;
using System.Collections.Generic;

namespace PemitManagement.Models;

public partial class PermitTypeIssuer
{
    public int Id { get; set; }

    public int PermitTypeId { get; set; }

    public int IssuerRoleId { get; set; }

    public int Sequence { get; set; }

    public bool? IsOptional { get; set; }

    public bool? Active { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
