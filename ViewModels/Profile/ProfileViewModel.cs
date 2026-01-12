namespace PemitManagement.ViewModels.Profile;

public class ProfileViewModel
{
    // Employee info
    public int EmployeeId { get; set; }
    public string EmpNo { get; set; } = "";
    public string Name { get; set; } = "";
    //public string Department { get; set; } = "";
    //public string Designation { get; set; } = "";
    public string Email { get; set; } = "";
    public string LastLogin { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Permissions
    public List<ProfilePermissionVM> Permissions { get; set; } = new();

    // Password change
    public string? CurrentPassword { get; set; }
    public string? NewPassword { get; set; }
    public string? ConfirmPassword { get; set; }
}
public class ProfilePermissionVM
{
    public string Key { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public bool HasPermission { get; set; }
}
