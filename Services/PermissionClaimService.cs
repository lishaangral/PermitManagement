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

    public async Task SyncClaimsAsync(ApplicationUser user)
    {
        var existingClaims = await _userManager.GetClaimsAsync(user);
        var permissionClaims = existingClaims
            .Where(c => c.Type == "permission")
            .ToList();

        foreach (var c in permissionClaims)
            await _userManager.RemoveClaimAsync(user, c);

        var permissions = await _db.UserPermissions
            .Where(up => up.UserId == user.EmployeeId)
            .Select(up => up.Permission.Name)
            .ToListAsync();

        foreach (var p in permissions)
        {
            await _userManager.AddClaimAsync(
                user,
                new Claim("permission", p)
            );
        }
    }
}
