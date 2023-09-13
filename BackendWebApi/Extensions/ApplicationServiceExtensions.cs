using BackendWebApi.Database;
using BackendWebApi.Helpers;
using BackendWebApi.Interfaces;
using BackendWebApi.Services;
using BackendWebApi.SignalR;
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
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<TrackUserActivity>();
            services.AddSignalR();
            services.AddSingleton<ActiveStatusTracker>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}