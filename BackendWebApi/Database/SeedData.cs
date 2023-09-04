using System.Text.Json;
using BackendWebApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BackendWebApi.Database
{
    public class SeedData
    {
        public static async Task SeedUsersData(UserManager<User> userManager)
        {
            if(await userManager.Users.AnyAsync()) return;

            var userData = await File.ReadAllTextAsync("Database/UserSeedData.json");
            var options = new JsonSerializerOptions{PropertyNameCaseInsensitive = true};
            var users = JsonSerializer.Deserialize<List<User>>(userData);

            foreach (var user in users)
            {
                user.UserName = user.UserName.ToLower();
                await userManager.CreateAsync(user, "Pa$$w0rd");
            }
        }
    }
}