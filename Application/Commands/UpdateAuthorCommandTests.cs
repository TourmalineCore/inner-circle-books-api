using Application;
using Application.Commands;
using Application.Requests;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class UpdateAuthorCommandTests
{
    private readonly AppDbContext _context;
    private readonly UpdateAuthorCommand _command;

    private const long TENANT_ID = 1L;

    public UpdateAuthorCommandTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "UpdateAuthorCommandBooksDatabase")
            .Options;

        _context = new AppDbContext(options);
        _command = new UpdateAuthorCommand(_context);
    }

    [Fact]
    public async Task UpdateAsync_ShouldSetDifferentValues()
    {
        var author = new Author(TENANT_ID, "Test Author")
        {
            Id = 1L,
            Books = new List<Book>()
        };
        var book = new Book
        {
            Id = 1L,
            TenantId = TENANT_ID,
            Title = "Test Book",
            Annotation = "Test annotation",
            Language = Language.English,
            Authors = new List<Author>(),
            ArtworkUrl = "http://test-images.com/img404.png",
        };
        author.Books.Add(book);
        book.Authors.Add(author);

        var updateAuthorRequest = new UpdateAuthorRequest()
        {
            FullName = "New Test Author FullName"
        };
        _context.Books.Add(book);
        _context.Authors.Add(author);
        await _context.SaveChangesAsync();

        await _command.UpdateAsync(1L, updateAuthorRequest, TENANT_ID);

        var updatedAuthor = await _context.Authors.FindAsync(author.Id);
        Assert.Equal(updateAuthorRequest.FullName, updatedAuthor.FullName);
    }
}