using Application;
using Application.Commands;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class EditBookCommandTests
{
    private const long TENANT_ID = 1;
    private readonly EditBookCommand _command;
    private readonly AppDbContext _context;

    public EditBookCommandTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("EditBookCommandBooksDatabase")
            .Options;

        _context = new AppDbContext(options);
        _command = new EditBookCommand(_context);
    }

    [Fact]
    public async Task EditAsync_ShouldSetDifferentValues()
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
            CoverUrl = "http://test-images.com/img404.png"
        };

        var editBookRequest = new EditBookCommandParams
        {
            Title = "Another title",
            Annotation = "Another annotation",
            Authors = new List<Author>()
            {
                new Author()
                {
                    FullName = "Editd Test Author"
                }
            },
            Language = 0
        };
        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        await _command.EditAsync(1, editBookRequest, TENANT_ID);

        var editdBook = await _context.Books.FindAsync(book.Id);
        Assert.Equal(editdBook.Title, editdBook.Title);
        Assert.Equal(editdBook.Annotation, editdBook.Annotation);
        Assert.Contains(editdBook.Authors, x => x.FullName == "Editd Test Author");
    }
}