using System;
using System.Collections.Generic;

namespace PemitManagement.Models;

public partial class UserPermission
{
    public uint Id { get; set; }

    public int UserId { get; set; }

    public uint PermissionId { get; set; }

    public int? GrantedBy { get; set; }

    public DateTime GrantedAt { get; set; }

    public virtual Employee? GrantedByNavigation { get; set; }

    public virtual Permission Permission { get; set; } = null!;

    public virtual Employee User { get; set; } = null!;

    public bool? Active { get; set; }
}
