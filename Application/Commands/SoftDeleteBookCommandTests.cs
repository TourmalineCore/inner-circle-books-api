using Application;
using Application.Commands;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class SoftDeleteBookCommandTests
{
    private const long TENANT_ID = 1;
    private readonly SoftDeleteBookCommand _command;
    private readonly AppDbContext _context;

    public SoftDeleteBookCommandTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("SoftDeleteBookCommandBooksDatabase")
            .Options;

        _context = new AppDbContext(options);
        _command = new SoftDeleteBookCommand(_context);
    }

    [Fact]
    public async Task SoftDeleteAsync_ShouldSetDeletedAtUtcValue()
    {
        var book = new Book
        {
            Id = 1,
            TenantId = TENANT_ID,
            Title = "Test Book",
            Annotation = "Test annotation",
            Authors = new List<Author>()
            {
                new Author()
                {
                    FullName = "Test Author"
                }
            },
            Language = Language.en,
            BookCoverUrl = "http://test-images.com/img404.png"
        };

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        await _command.SoftDeleteAsync(book.Id, book.TenantId);

        var editdBook = await _context.Books.SingleAsync(x => x.Id == book.Id);

        Assert.NotNull(editdBook.DeletedAtUtc);
    }
}