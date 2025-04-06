using KingHotelProject.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace KingHotelProject.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Dish> Dishes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Hotel>()
                .HasMany(h => h.Dishes)
                .WithOne(d => d.Hotel)
                .HasForeignKey(d => d.HotelId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}