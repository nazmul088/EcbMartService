using Microsoft.EntityFrameworkCore;
using EcbMartService.Models;

namespace EcbMartService.Data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<DeliveryAddress> DeliveryAddresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User-DeliveryAddress relationship
            modelBuilder.Entity<DeliveryAddress>()
                .HasOne(d => d.User)
                .WithMany(u => u.DeliveryAddresses)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure unique constraint for default address per user
            modelBuilder.Entity<DeliveryAddress>()
                .HasIndex(d => new { d.UserId, d.IsDefault })
                .HasFilter("\"IsDefault\" = true")
                .IsUnique();
        }
    }
}
