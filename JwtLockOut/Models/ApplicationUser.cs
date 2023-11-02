using Microsoft.AspNetCore.Identity;

namespace JwtLockOut.Models
{
    public class ApplicationUser : IdentityUser
    {
        public required string Name { get; set; }
    }
}
