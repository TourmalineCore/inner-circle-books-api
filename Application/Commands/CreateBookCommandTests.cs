using Application.Requests;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Application.Commands;

public class CreateBookCommandTests
{
    private const long TENANT_ID = 1L;
    private readonly CreateBookCommand _command;
    private readonly AppDbContext _context;

    public CreateBookCommandTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("CreateBookCommandBooksDatabase")
            .Options;

        _context = new AppDbContext(options);
        var createAuthorCommand = new CreateAuthorCommand(_context);
        _command = new CreateBookCommand(_context, createAuthorCommand);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddNewBookToDbSet()
    {
        var createBookRequest = new CreateBookRequest
        {
            Title = "Test Book",
            Annotation = "Test annotation",
            Language = "Russian",
            ArtworkUrl = "http://test-images.com/img404.png",
            Authors = new List<string>
            {
                "Test Author"
            }
        };

        var bookId = await _command.CreateAsync(createBookRequest, TENANT_ID);

        var book = await _context.Books.FindAsync(bookId);
        Assert.NotNull(book);
        Assert.Equal("Test Book", book.Title);
        Assert.Equal(bookId, book.Id);
    }
}