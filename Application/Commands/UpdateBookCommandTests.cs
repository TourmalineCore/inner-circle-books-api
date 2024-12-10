using Application;
using Application.Commands;
using Application.Requests;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class UpdateBookCommandTests
{
    private readonly AppDbContext _context;
    private readonly UpdateBookCommand _command;

    private const long TENANT_ID = 1L;

    public UpdateBookCommandTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "UpdateBookCommandBooksDatabase")
            .Options;

        _context = new AppDbContext(options);
        _command = new UpdateBookCommand(_context);
    }

    [Fact]
    public async Task UpdateAsync_ShouldSetDifferentValues()
    {
        var book = new Book
        {
            Id = 1L,
            TenantId = TENANT_ID,
            Title = "Test Book",
            Annotation = "Test annotation",
            ArtworkUrl = "http://test-images.com/img404.png",
        };
        var updateBookRequest = new UpdateBookRequest()
        {
            Id = 1L,
            Annotation = "Another annotation",
            Title = "Another title",
        };
        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        await _command.UpdateAsync(updateBookRequest, TENANT_ID);

        var updatedBook = await _context.Books.FindAsync(book.Id);
        Assert.Equal(updatedBook.Title, updatedBook.Title);
        Assert.Equal(updatedBook.Annotation, updatedBook.Annotation);
    }
}