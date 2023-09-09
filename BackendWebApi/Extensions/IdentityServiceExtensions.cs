using System.Text;
using BackendWebApi.Database;
using BackendWebApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace BackendWebApi.Extensions;

public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentityCore<User>(opt => 
        {
            opt.Password.RequireNonAlphanumeric = false;
        }).AddRoles<Role>().AddRoleManager<RoleManager<Role>>().AddEntityFrameworkStores<DataContext>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => 
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["TokenKey"])),
                ValidateIssuer = false,
                ValidateAudience = false
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context => 
                {
                    var accessToken = context.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path;

                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                    {
                        context.Token = accessToken;
                    }

                    return Task.CompletedTask;
                }
            };
        });

        services.AddAuthorization(opt => {
            opt.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
            opt.AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin", "Moderator"));
        });

        return services;
    }
}