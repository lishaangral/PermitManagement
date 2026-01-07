using System.ComponentModel.DataAnnotations;

namespace PemitManagement.ViewModels.Violations
{
    public class EditViolationViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = "";

        public string? Description { get; set; }

        public string? Category { get; set; }

        [Required]
        public string Severity { get; set; } = "low";

        public bool Active { get; set; }

        public List<int> PermitTypeIds { get; set; } = new();
    }
}
