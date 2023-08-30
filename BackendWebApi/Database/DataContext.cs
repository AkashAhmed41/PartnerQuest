using BackendWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendWebApi.Database
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<FavouriteUsers> FavouriteUsersDb { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

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