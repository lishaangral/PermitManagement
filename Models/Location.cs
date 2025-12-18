using System;
using System.Collections.Generic;

namespace PemitManagement.Models;

public partial class Location
{
    public uint Id { get; set; }

    public string Name { get; set; } = null!;

    public string? RefineryType { get; set; }

    public bool? Active { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Permit> Permits { get; set; } = new List<Permit>();
}
