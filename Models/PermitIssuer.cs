using System;
using System.Collections.Generic;

namespace PemitManagement.Models;

public partial class PermitIssuer
{
    public int Id { get; set; }

    public int PermitId { get; set; }

    public int IssuerRoleId { get; set; }

    public string EmployeeId { get; set; } = null!;

    public string EmployeeName { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}
