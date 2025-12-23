using System.ComponentModel.DataAnnotations;

namespace PemitManagement.ViewModels;

public class LoginViewModel
{
    [Required]
    [Display(Name = "Employee ID")]
    public string EmpNo { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
