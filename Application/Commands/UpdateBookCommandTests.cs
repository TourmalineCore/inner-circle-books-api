using Application;
using Application.Commands;
using Application.Requests;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class UpdateBookCommandTests
{
    private const long TENANT_ID = 1L;
    private readonly UpdateBookCommand _command;
    private readonly AppDbContext _context;

    public UpdateBookCommandTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("UpdateBookCommandBooksDatabase")
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

        var updateBookRequest = new UpdateBookRequest
        {
            Title = "Another title",
            Annotation = "Another annotation",
            Authors = new List<AuthorModel>()
            {
                new AuthorModel()
                {
                    FullName = "Updated Test Author"
                }
            },
            Language = "ru"
        };
        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        await _command.UpdateAsync(1, updateBookRequest, TENANT_ID);

        var updatedBook = await _context.Books.FindAsync(book.Id);
        Assert.Equal(updatedBook.Title, updatedBook.Title);
        Assert.Equal(updatedBook.Annotation, updatedBook.Annotation);
        Assert.Contains(updatedBook.Authors, x => x.FullName == "Updated Test Author");
    }
}