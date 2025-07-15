using LibraryManagement.Domain.Entities;
using LibraryManagement.Infrastructure.Data;
using LibraryManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BookRepositoryTests
{
    public class BookRepositoryTests
    {
        private AppDbContext GetInMemoryContext()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase($"Db_{Guid.NewGuid()}")
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task AddAsync_ShouldAddBook()
        {
            using var context = GetInMemoryContext();
            var repo = new BookRepository(context);

            var book = new Book
            {
                Title = "Test Book",
                Author = "Author A",
                ISBN = "9781617294532"
            };
            await repo.AddAsync(book);

            var allBooks = await repo.GetAllAsync(1, 10);
            Assert.Single(allBooks);
            Assert.Equal("Test Book", allBooks[0].Title);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnPaginatedBooks()
        {
            using var context = GetInMemoryContext();
            var repo = new BookRepository(context);

            for (int i = 1; i <= 20; i++)
            {
                await repo.AddAsync(new Book
                {
                    Title = $"Book {i}",
                    Author = "A",
                    ISBN = $"ISBN-{i:D3}"
                });
            }

            var page2 = await repo.GetAllAsync(2, 5);
            Assert.Equal(5, page2.Count);
            Assert.Equal("Book 6", page2[0].Title);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveBook()
        {
            using var context = GetInMemoryContext();
            var repo = new BookRepository(context);

            var book = new Book
            {
                Title = "Delete Me",
                Author = "Author B",
                ISBN = "9783161484100"
            };
            await repo.AddAsync(book);

            await repo.DeleteAsync(book.Id);
            var count = await repo.GetTotalCountAsync();

            Assert.Equal(0, count);
        }

        [Fact]
        public async Task DeleteAsync_NonExistentBook_ShouldNotThrow()
        {
            using var context = GetInMemoryContext();
            var repo = new BookRepository(context);

            int nonExistentBookId = -1; 

            await repo.DeleteAsync(nonExistentBookId);

            var count = await repo.GetTotalCountAsync();
            Assert.Equal(0, count);
        }


        [Fact]
        public async Task AddAsync_WithoutISBN_ShouldThrowException()
        {
            using var context = GetInMemoryContext();
            var repo = new BookRepository(context);

            var book = new Book
            {
                Title = "No ISBN Book",
                Author = "Author X",
                ISBN = null 
            };

            await Assert.ThrowsAsync<DbUpdateException>(async () =>
            {
                await repo.AddAsync(book);
            });
        }
    }
}
