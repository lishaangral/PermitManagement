using System.ComponentModel.DataAnnotations;
namespace PemitManagement.ViewModels.Observations;

public class SubmitViolationsViewModel
{
    [Required]
    public uint PermitId { get; set; }

    [MinLength(1, ErrorMessage = "At least one violation must be selected.")]
    public List<SelectedViolationViewModel> Violations { get; set; } = new();
}
