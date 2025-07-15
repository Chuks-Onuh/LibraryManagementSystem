using LibraryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=library.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>().HasData(
                new Book
                {
                    Id = 1,
                    Title = "To Kill a Mockingbird",
                    Author = "Harper Lee",
                    ISBN = "9780061120084",
                    PublishedDate = new DateTime(1960, 7, 11),
                    DateCreated = DateTime.Now
                },
                new Book
                {
                    Id = 2,
                    Title = "1984",
                    Author = "George Orwell",
                    ISBN = "9780451524935",
                    PublishedDate = new DateTime(1949, 6, 8),
                    DateCreated = DateTime.Now
                },
                new Book
                {
                    Id = 3,
                    Title = "Pride and Prejudice",
                    Author = "Jane Austen",
                    ISBN = "9780141439518",
                    PublishedDate = new DateTime(1813, 1, 28),
                    DateCreated = DateTime.Now
                }
            );
        }

    }
}
