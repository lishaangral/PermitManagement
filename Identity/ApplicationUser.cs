using Microsoft.AspNetCore.Identity;

namespace PemitManagement.Identity
{
    public class ApplicationUser : IdentityUser
    {
        // Link to your domain user
        public int EmployeeId { get; set; }
    }
}
