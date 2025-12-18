using System;
using System.Collections.Generic;

namespace PemitManagement.Models;

public partial class ProjectStatus
{
    public uint Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Status { get; set; } = null!;

    public int Priority { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
