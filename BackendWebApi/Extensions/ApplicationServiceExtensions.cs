using BackendWebApi.Database;
using BackendWebApi.Interfaces;
using BackendWebApi.Services;
using Microsoft.EntityFrameworkCore;

namespace BackendWebApi.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DataContext>( opt => 
            {
                opt.UseSqlite(configuration.GetConnectionString("DefaultConnectionString"));
            });

            services.AddCors();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            return services;
        }
    }
}