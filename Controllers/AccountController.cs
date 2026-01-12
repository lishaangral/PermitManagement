using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PemitManagement.Data;
using PemitManagement.Identity;
using PemitManagement.Services;
using PemitManagement.ViewModels;
using PemitManagement.ViewModels.Profile;
using System.Security.Claims;
using PemitManagement.Authorization;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly PermissionClaimService _permissionClaimService;


    public AccountController(
        ApplicationDbContext db,
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        PermissionClaimService permissionClaimService)
    {
        _db = db;
        _signInManager = signInManager;
        _userManager = userManager;
        _permissionClaimService = permissionClaimService;
    }


    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var employee = await _db.Employees
            .FirstOrDefaultAsync(e => e.EmpNo == model.EmpNo && e.Active == true);

        if (employee == null || employee.Password != model.Password)
        {
            ModelState.AddModelError("", "Invalid credentials");
            return View(model);
        }

        var user = await _userManager.FindByNameAsync(model.EmpNo);
        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = model.EmpNo,
                EmployeeId = employee.Id
            };
            await _userManager.CreateAsync(user);
        }
        await _permissionClaimService.SyncPermissionsAsync(user);

        var permissions = await _db.UserPermissions
            .Where(up => up.UserId == employee.Id)
            .Select(up => up.Permission.Name)
            .ToListAsync();

        var claims = permissions
            .Select(p => new Claim("permission", p))
            .ToList();

        await _signInManager.SignInWithClaimsAsync(
            user,
            isPersistent: false,
            claims
        );

        await _signInManager.SignInAsync(user, false);

        TempData["ToastMessage"] = "Logged in successfully";
        return RedirectToAction("Index", "Dashboard");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        TempData["ToastMessage"] = "Logged out successfully";
        return RedirectToAction("Login", "Account");
    }  

    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var empNo = User.Identity?.Name;
        if (string.IsNullOrWhiteSpace(empNo))
            return RedirectToAction("Login");

        var employee = await _db.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.EmpNo == empNo);

        if (employee == null)
            return RedirectToAction("Login");

        var userPermissions = User.Claims
            .Where(c => c.Type == "permission")
            .Select(c => c.Value)
            .ToHashSet();

        var vm = new ProfileViewModel
        {
            EmployeeId = employee.Id,
            EmpNo = employee.EmpNo,
            Name = employee.Name,
            //Department = employee.Department ?? "—",
            //Designation = employee.Designation ?? "—",
            Email = employee.Email ?? "—",
            LastLogin = employee.LastLogin ?? "—",
            CreatedAt = employee.CreatedAt,
            UpdatedAt = employee.UpdatedAt,
            Permissions = PermissionConstants.All
                .Select(p => new ProfilePermissionVM
                {
                    Key = p,
                    DisplayName = p.Replace("_", " ")
                                   .Split(' ')
                                   .Select(w => char.ToUpper(w[0]) + w.Substring(1))
                                   .Aggregate((a, b) => $"{a} {b}"),
                    HasPermission = userPermissions.Contains(p)
                                    || p == PermissionConstants.ViewReports
                })
                .ToList()
        };

        return View(vm);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ProfileViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.CurrentPassword) ||
            string.IsNullOrWhiteSpace(model.NewPassword))
        {
            TempData["ToastMessage"] = "Password fields cannot be empty.";
            return RedirectToAction(nameof(Profile));
        }

        if (model.NewPassword != model.ConfirmPassword)
        {
            TempData["ToastMessage"] = "Passwords do not match.";
            return RedirectToAction(nameof(Profile));
        }

        var empNo = User.Identity?.Name;
        var employee = await _db.Employees.FirstOrDefaultAsync(e => e.EmpNo == empNo);

        if (employee == null || employee.Password != model.CurrentPassword)
        {
            TempData["ToastMessage"] = "Current password is incorrect.";
            return RedirectToAction(nameof(Profile));
        }

        employee.Password = model.NewPassword;
        employee.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        TempData["ToastMessage"] = "Password updated successfully.";
        return RedirectToAction(nameof(Profile));
    }

}

