using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Application.Commands;

public class CreateBookCommandTests
{
    private const long TENANT_ID = 1;
    private readonly CreateBookCommand _command;
    private readonly AppDbContext _context;

    public CreateBookCommandTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("CreateBookCommandBooksDatabase")
            .Options;

        _context = new AppDbContext(options);
        _command = new CreateBookCommand(_context);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddNewBookToDbSet()
    {
        var createBookRequest = new CreateBookCommandParams
        {
            Title = "Test Book",
            Annotation = "Test annotation",
            Language = 0,
            CoverUrl = "http://test-images.com/img404.png",
            Authors = new List<Author>
            {
                new Author()
                {
                    FullName = "Test Author"
                }
            },
            CountOfCopies = 2
        };

        var bookId = await _command.CreateAsync(createBookRequest, TENANT_ID);

        // Verify the book is added correctly
        var book = await _context.Books.FindAsync(bookId);
        Assert.NotNull(book);
        Assert.Equal("Test Book", book.Title);
        Assert.Equal(bookId, book.Id);

        // Verify the book copies are added correctly
        var copies = await _context.BooksCopies
            .Where(copy => copy.TenantId == TENANT_ID)
            .Where(copy => copy.BookId == bookId)
            .ToListAsync();

        Assert.NotNull(copies);
        Assert.Equal(createBookRequest.CountOfCopies, copies.Count);

        foreach (var copy in copies)
        {
            Assert.Equal(bookId, copy.BookId);
            Assert.NotNull(copy.SecretKey);
        }
    }

    [Fact]
    public async Task CreateWithouAuthorsAsync_ShouldThrowException()
    {
        var createBookRequest = new CreateBookCommandParams
        {
            Title = "Test Book",
            Annotation = "Test annotation",
            Language = 0,
            CoverUrl = "http://test-images.com/img404.png",
            Authors = new List<Author>
            {
            }
        };

        await Assert.ThrowsAsync<ArgumentException>(async () => await _command.CreateAsync(createBookRequest, TENANT_ID));
    }
}