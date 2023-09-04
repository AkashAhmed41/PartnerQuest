using Microsoft.AspNetCore.Identity;

namespace BackendWebApi.Models;

public class UserRole : IdentityUserRole<int>
{
    public User User { get; set; }
    public Role Role { get; set; }
}