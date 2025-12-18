using System;
using System.Collections.Generic;

namespace PemitManagement.Models;

public partial class CustomIssuerField
{
    public int Id { get; set; }

    public string FieldName { get; set; } = null!;

    public string FieldKey { get; set; } = null!;

    public bool? IsRequired { get; set; }

    public bool? IsActive { get; set; }

    public int? DisplayOrder { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
