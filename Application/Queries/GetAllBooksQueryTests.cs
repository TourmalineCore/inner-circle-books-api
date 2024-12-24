using Application;
using Application.Queries;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class GetAllBooksQueryTests
{
    private const long TENANT_ID = 1L;
    private readonly AppDbContext _context;
    private readonly GetAllBooksQuery _query;

    public GetAllBooksQueryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("GetAllBooksQueryBooksDatabase")
            .Options;

        _context = new AppDbContext(options);
        _query = new GetAllBooksQuery(_context);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllBooks()
    {
        var book1 = new Book
        {
            Id = 1L,
            TenantId = TENANT_ID,
            Title = "Test Book 1",
            Annotation = "Test annotation 1",
            Authors = new List<Author>()
            {
                new Author()
                {
                    FullName = "Test Author"
                }
            },
            ArtworkUrl = "http://test1-images.com/img404.png"
        };
        var book2 = new Book
        {
            Id = 2L,
            TenantId = TENANT_ID,
            Title = "Test Book 2",
            Annotation = "Test annotation 2",
            Authors = new List<Author>()
            {
                new Author()
                {
                    FullName = "Test Author 2"
                }
            },
            ArtworkUrl = "http://test2-images.com/img405.png"
        };
        var book3 = new Book
        {
            Id = 3L,
            TenantId = TENANT_ID,
            Title = "Test Book 3",
            Annotation = "Test annotation 3",
            Authors = new List<Author>()
            {
                new Author()
                {
                    FullName = "Test Author 3"
                }
            },
            ArtworkUrl = "http://test3-images.com/img406.png"
        };

        _context.Books.AddRange(book1, book2, book3);
        await _context.SaveChangesAsync();

        var result = await _query.GetAllAsync(TENANT_ID);

        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Contains(result, t => t.Id == 1L && t.Title == "Test Book 1");
        Assert.Contains(result, t => t.Id == 2L && t.Title == "Test Book 2");
        Assert.Contains(result, t => t.Id == 3L && t.Title == "Test Book 3");
    }
}