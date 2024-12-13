using Application;
using Application.Commands;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class DeleteAuthorCommandTests
{
    private const long TENANT_ID = 1L;
    private readonly DeleteAuthorCommand _command;
    private readonly AppDbContext _context;

    public DeleteAuthorCommandTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("DeleteAuthorCommandBooksDatabase")
            .Options;

        _context = new AppDbContext(options);
        _command = new DeleteAuthorCommand(_context);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteAuthorFromBookAndFromDbSet()
    {
        var author = new Author(TENANT_ID, "Test author")
        {
            Id = 1L,
            Books = new List<Book>()
        };
        var author2 = new Author(TENANT_ID, "Test author 2")
        {
            Id = 2L,
            Books = new List<Book>()
        };
        var book = new Book
        {
            Id = 1L,
            TenantId = TENANT_ID,
            Title = "Test Book",
            Annotation = "Test annotation",
            Authors = new List<Author>(),
            ArtworkUrl = "http://test-images.com/img404.png"
        };

        book.Authors.Add(author);
        book.Authors.Add(author2);
        author.Books.Add(book);
        author2.Books.Add(book);

        _context.Books.Add(book);
        _context.Authors.Add(author);
        await _context.SaveChangesAsync();

        await _command.DeleteAsync(author.Id, author.TenantId);

        Assert.DoesNotContain(author, book.Authors);
    }
}