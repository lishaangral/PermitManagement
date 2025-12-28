using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace PemitManagement.ViewModels.Observations;

public class SelectedViolationViewModel
{
    [Required]
    public uint ViolationId { get; set; }
    public string Name { get; set; } = "";
    public string Category { get; set; } = "";
    public string Severity { get; set; } = "";

    [MaxLength(2000)]
    public string? Remarks { get; set; }

    [MaxLength(2000)]
    public string? ActionTaken { get; set; }

    public List<IFormFile> Images { get; set; } = new();
}