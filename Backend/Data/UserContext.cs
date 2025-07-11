using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data
{
    public class UserContext(DbContextOptions<UserContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
            });


            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasIndex(e => e.BookName);
            });
        }

    }
}
