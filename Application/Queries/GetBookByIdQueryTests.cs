using Application;
using Application.Queries;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class GetBookByIdQueryTests
{
    private const long TENANT_ID = 1;
    private readonly AppDbContext _context;
    private readonly GetBookByIdQuery _query;

    public GetBookByIdQueryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("GetBookByIdQueryBooksDatabase")
            .Options;

        _context = new AppDbContext(options);
        _query = new GetBookByIdQuery(_context);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnBook_WhenBookExists()
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

        var result = await _query.GetByIdAsync(book.Id, TENANT_ID);

        Assert.NotNull(result);
        Assert.Equal(book.Id, result.Id);
        Assert.Equal(book.Title, result.Title);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenBookDoesNotExist()
    {
        var nonExistentId = 999;

        var result = await _query.GetByIdAsync(nonExistentId, TENANT_ID);

        Assert.Null(result);
    }
}