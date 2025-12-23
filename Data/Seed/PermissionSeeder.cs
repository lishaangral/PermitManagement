using PemitManagement.Authorization;
using PemitManagement.Data;
using PemitManagement.Models;

namespace PemitManagement.Data.Seed
{
    public static class PermissionSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext db)
        {
            foreach (var permissionName in PermissionConstants.All)
            {
                if (!db.Permissions.Any(p => p.Name == permissionName))
                {
                    db.Permissions.Add(new Permission
                    {
                        Name = permissionName,
                        Description = permissionName.Replace("_", " "),
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            await db.SaveChangesAsync();
        }
    }
}
