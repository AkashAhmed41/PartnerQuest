using Microsoft.AspNetCore.Identity;
namespace BackendWebApi.Models;

public class Role : IdentityRole<int>
{
    public ICollection<UserRole> UserRoles { get; set; }
}