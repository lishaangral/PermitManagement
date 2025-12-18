using System;
using System.Collections.Generic;

namespace PemitManagement.Models;

public partial class EmployeePermission
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public bool? CanManageUsers { get; set; }

    public bool? CanViewReports { get; set; }

    public bool? CanEditLocations { get; set; }

    public bool? CanEditPermitTypes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool CanCreatePermits { get; set; }

    public bool CanAccessDashboard { get; set; }

    public bool CanAccessMyPermits { get; set; }

    public virtual Employee Employee { get; set; } = null!;
}
