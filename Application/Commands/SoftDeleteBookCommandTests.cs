using Application;
using Application.Commands;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Xunit;

public class SoftDeleteBookCommandTests
{
    private readonly AppDbContext _context;
    private readonly SoftDeleteBookCommand _command;

    public SoftDeleteBookCommandTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "SoftDeleteBookCommandBooksDatabase")
            .Options;

        _context = new AppDbContext(options);
        _command = new SoftDeleteBookCommand(_context);
    }

    [Fact]
    public async Task SoftDeleteAsync_ShouldSetDeletedAtUtc()
    {
        var currentInstant = Instant.FromUtc(2024, 8, 28, 12, 0, 0);

        var book = new Book
        {
            Id = 1,
            Title = "Test Book",
            Annotation = "Test annotation",
            ArtworkUrl = "http://test-images.com/img404.png",
        };
        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        await _command.SoftDeleteAsync(book.Id);

        var updatedBook = await _context.Books.FindAsync(book.Id);
        Assert.NotNull(updatedBook.DeletedAtUtc);
    }
}