using System.Text.Json;
using BackendWebApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BackendWebApi.Database;

public class SeedData
{
    public static async Task SeedUsersData(UserManager<User> userManager, RoleManager<Role> roleManager)
    {
        if(await userManager.Users.AnyAsync()) return;

        var userData = await File.ReadAllTextAsync("Database/UserSeedData.json");
        var options = new JsonSerializerOptions{PropertyNameCaseInsensitive = true};
        var users = JsonSerializer.Deserialize<List<User>>(userData);
        var roles = new List<Role>
        {
            new() { Name = "Member"},
            new() { Name = "Admin"},
            new() { Name = "Moderator"}
        };

        foreach (var role in roles)
        {
            await roleManager.CreateAsync(role);
        }

        foreach (var user in users)
        {
            user.UserName = user.UserName.ToLower();
            await userManager.CreateAsync(user, "Pa$$w0rd");
            await userManager.AddToRoleAsync(user, "Member");
        }

        var admin = new User
        {
            UserName = "admin",
            Nickname = "Administrator"
        };
        await userManager.CreateAsync(admin, "Pa$$w0rd");
        await userManager.AddToRolesAsync(admin, new[] {"Admin", "Moderator"});
    }
}