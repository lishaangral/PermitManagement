using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PemitManagement.Data;
using PemitManagement.Identity;

namespace PemitManagement.Services;

public class PermissionClaimService
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public PermissionClaimService(
        ApplicationDbContext db,
        UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task SyncPermissionsAsync(ApplicationUser user)
    {
        // Remove old permission claims
        var existingClaims = await _userManager.GetClaimsAsync(user);
        var permissionClaims = existingClaims
            .Where(c => c.Type == "permission")
            .ToList();

        foreach (var claim in permissionClaims)
        {
            await _userManager.RemoveClaimAsync(user, claim);
        }

        // Fetch permissions from DB (employee-based)
        var permissions = await _db.UserPermissions
            .Where(up =>
                up.UserId == user.EmployeeId &&
                up.Active == true
            )
            .Select(up => up.Permission.Name)
            .ToListAsync();

        // Add permission claims
        foreach (var permission in permissions)
        {
            await _userManager.AddClaimAsync(
                user,
                new Claim("permission", permission)
            );
        }
    }
}
