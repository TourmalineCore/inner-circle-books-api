using Application;
using Application.Commands;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class DeleteBookCommandTests
{
    private const long TENANT_ID = 1L;
    private readonly DeleteBookCommand _command;
    private readonly AppDbContext _context;

    public DeleteBookCommandTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("DeleteBookCommandBooksDatabase")
            .Options;

        _context = new AppDbContext(options);
        _command = new DeleteBookCommand(_context);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteBookFromDbSet()
    {
        var book = new Book
        {
            Id = 1L,
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
            ArtworkUrl = "http://test-images.com/img404.png"
        };

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        await _command.DeleteAsync(book.Id, book.TenantId);

        var isBookExistInDbSet = await _context.Books.AnyAsync(x => x.Id == book.Id);
        Assert.False(isBookExistInDbSet);
    }
}