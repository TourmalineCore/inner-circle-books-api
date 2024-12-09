using Xunit;
using Moq;
using Application.Requests;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace Application.Commands.Tests
{
    public class CreateBookCommandTests
    {
        private readonly AppDbContext _context;
        private readonly CreateBookCommand _command;

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
                ArtworkUrl = "http://test-images.com/img404.png",
                AuthorId = 1L,
                LanguageId = 1L,
                NumberOfCopies = 1
            };

            var bookId = await _command.CreateAsync(addBookRequest);

            var book = await _context.Books.FindAsync(bookId);
            Assert.NotNull(book);
            Assert.Equal("Test book", book.Title);
            Assert.Equal(bookId, book.Id);
        }
    }
}
