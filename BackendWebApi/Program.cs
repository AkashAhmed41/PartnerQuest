using BackendWebApi.Database;
using BackendWebApi.Extensions;
using BackendWebApi.Middleware;
using BackendWebApi.Models;
using BackendWebApi.SignalR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("http://localhost:4200"));

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<PresenceHub>("hubs/presence");
app.MapHub<MessagesHub>("hubs/messages");

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try 
{
    var context = services.GetRequiredService<DataContext>();
    var userManager = services.GetRequiredService<UserManager<User>>();
    var roleManager = services.GetRequiredService<RoleManager<Role>>();
    await context.Database.MigrateAsync();
    await context.Database.ExecuteSqlRawAsync("DELETE FROM [Connections]");
    await SeedData.SeedUsersData(userManager, roleManager);
} catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred during migration!");
}

app.Run();
