using Application;
using Application.Commands;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class SoftDeleteBookCommandTests
{
    private const long TENANT_ID = 1L;
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
    public async Task SoftDeleteAsync_ShouldSetDeletedAtUtc()
    {
        var book = new Book
        {
            Id = 1,
            TenantId = TENANT_ID,
            Title = "Test Book",
            Annotation = "Test annotation",
            ArtworkUrl = "http://test-images.com/img404.png"
        };
        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        await _command.SoftDeleteAsync(book.Id, book.TenantId);

        var updatedBook = await _context.Books.FindAsync(book.Id);
        Assert.NotNull(updatedBook.DeletedAtUtc);
    }
}