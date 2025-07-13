using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Book> Books { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
            });


            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasIndex(e => e.BookName);

                entity.Property(e => e.Price)
                  .HasColumnType("decimal(18,2)");

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UpdatedAt)
                    .IsRequired(false);
            });
        }

    }
}
