using System;
using System.Collections.Generic;

namespace PemitManagement.Models;

public partial class UserPermission
{
    public uint Id { get; set; }

    public uint UserId { get; set; }

    public uint PermissionId { get; set; }

    public uint GrantedBy { get; set; }

    public DateTime GrantedAt { get; set; }

    public virtual Admin GrantedByNavigation { get; set; } = null!;

    public virtual Permission Permission { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
