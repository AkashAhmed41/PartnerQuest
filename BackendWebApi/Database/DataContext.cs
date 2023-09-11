using BackendWebApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BackendWebApi.Database
{
    public class DataContext : IdentityDbContext<User, Role, int, IdentityUserClaim<int>, 
                                UserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<FavouriteUsers> FavouriteUsersDb { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<SignalRGroup> MessagesGroups { get; set; }
        public DbSet<SignalRConnection> Connections { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>()
                .HasMany(user => user.UserRoles)
                .WithOne(userRole => userRole.User)
                .HasForeignKey(user => user.UserId)
                .IsRequired();

            builder.Entity<Role>()
                .HasMany(role => role.UserRoles)
                .WithOne(user => user.Role)
                .HasForeignKey(user => user.RoleId)
                .IsRequired();

            builder.Entity<FavouriteUsers>().HasKey(obj => new {obj.SourceUserId, obj.FavouriteUserId});

            builder.Entity<FavouriteUsers>()
                .HasOne(s => s.SourceUser)
                .WithMany(u => u.AddedFavouriteUsers)
                .HasForeignKey(s => s.SourceUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<FavouriteUsers>()
                .HasOne(f => f.FavouriteUser)
                .WithMany(u => u.AddedFavouriteBy)
                .HasForeignKey(f => f.FavouriteUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Message>()
                .HasOne(u => u.Sender)
                .WithMany(msg => msg.MessagesSent)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(u => u.Recipient)
                .WithMany(msg => msg.MessagesReceived)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}