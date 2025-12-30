using System.ComponentModel.DataAnnotations;
using PemitManagement.Data.Enums;

namespace PemitManagement.ViewModels.Violations
{
    public class CreateViolationViewModel
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? Category { get; set; }

        [Required]
        public string Severity { get; set; } = "";

        public bool Active { get; set; } = true;

        public List<int>? PermitTypeIds { get; set; }
    }
}
