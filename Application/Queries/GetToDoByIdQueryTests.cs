using Application;
using Application.Queries;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class GetToDoByIdQueryTests
{
    private const long TENANT_ID = 1L;
    private readonly AppDbContext _context;
    private readonly GetBookByIdQuery _query;

    public GetToDoByIdQueryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("GetBookByIdQueryBooksDatabase")
            .Options;

        _context = new AppDbContext(options);
        _query = new GetBookByIdQuery(_context);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnToDo_WhenToDoExists()
    {
        var book = new Book
        {
            Id = 1,
            TenantId = TENANT_ID,
            Title = "Test Book",
            Annotation = "Test annotation",
            ArtworkUrl = "http://test-images.com/img404.png",
            Authors = new List<Author>
            {
                new(TENANT_ID, "Test Author")
            }
        };
        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        var result = await _query.GetByIdAsync(book.Id, TENANT_ID);

        Assert.NotNull(result);
        Assert.Equal(book.Id, result.Id);
        Assert.Equal(book.Title, result.Title);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenToDoDoesNotExist()
    {
        var nonExistentId = 999;

        var result = await _query.GetByIdAsync(nonExistentId, TENANT_ID);

        Assert.Null(result);
    }
}