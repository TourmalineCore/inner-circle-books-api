using Application;
using Application.Commands;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class DeleteBookCommandTests
{
    private readonly AppDbContext _context;
    private readonly DeleteBookCommand _command;

    public DeleteBookCommandTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "DeleteBookCommandBooksDatabase")
            .Options;

        _context = new AppDbContext(options);
        _command = new DeleteBookCommand(_context);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteBookFromDbSet()
    {
        var book = new Book
        {
            Id = 1,
            Title = "Test Book",
            Annotation = "Test annotation",
            ArtworkUrl = "http://test-images.com/img404.png",
        };
        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        await _command.DeleteAsync(book.Id);

        var deletedToDo = await _context.Books.FindAsync(book.Id);
        Assert.Null(deletedToDo);
    }
}