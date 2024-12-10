using Application.Requests;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Application.Commands.Tests
{
    public class CreateBookCommandTests
    {
        private readonly AppDbContext _context;
        private readonly CreateBookCommand _command;

        private const long TENANT_ID = 1L;

        public CreateBookCommandTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "CreateBookCommandBooksDatabase")
                .Options;

            _context = new AppDbContext(options);
            _command = new CreateBookCommand(_context);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddNewBookToDbSet()
        {
            var addBookRequest = new AddBookRequest
            {
                Title = "Test Book",
                Annotation = "Test annotation",
                Language = "Russian",
                ArtworkUrl = "http://test-images.com/img404.png",
                Authors = new List<string>()
                {
                    "Test Author"
                },
            };

            var bookId = await _command.CreateAsync(addBookRequest, TENANT_ID);

            var book = await _context.Books.FindAsync(bookId);
            Assert.NotNull(book);
            Assert.Equal("Test Book", book.Title);
            Assert.Equal(bookId, book.Id);
        }
    }
}