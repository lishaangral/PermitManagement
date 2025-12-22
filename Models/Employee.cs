using System;
using System.Collections.Generic;

namespace PemitManagement.Models;

public partial class Employee
{
    public int Id { get; set; }

    public string EmpNo { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Department { get; set; }

    public string? Designation { get; set; }

    public bool? Active { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string? Password { get; set; }

    public string? Email { get; set; }

    public string? LastLogin { get; set; }

    public string? EmployeeRole { get; set; }

    public virtual ICollection<UserPermission> UserPermissionGrantedByNavigations { get; set; } = new List<UserPermission>();

    public virtual ICollection<UserPermission> UserPermissionUsers { get; set; } = new List<UserPermission>();
}
