using Xunit;
using Application;
using Application.Queries;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
public class GetAllToDosQueryTests
{
    private readonly AppDbContext _context;
    private readonly GetAllBooksQuery _query;

    public GetAllToDosQueryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "GetAllBooksQueryBooksDatabase")
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
            Title = "Test Book 1",
            Annotation = "Test annotation 1",
            ArtworkUrl = "http://test1-images.com/img404.png",
            AuthorId = 1L,
            NumberOfCopies = 1
        };
        var book2 = new Book
        {
            Id = 2L,
            Title = "Test Book 2",
            Annotation = "Test annotation 2",
            ArtworkUrl = "http://test2-images.com/img404.png",
            AuthorId = 2L,
            NumberOfCopies = 1
        };
        var book3 = new Book
        {
            Id = 3L,
            Title = "Test Book 3",
            Annotation = "Test annotation 3",
            ArtworkUrl = "http://test3-images.com/img404.png",
            AuthorId = 3L,
            NumberOfCopies = 2
        };

        _context.Books.AddRange(book1, book2, book3);
        await _context.SaveChangesAsync();

        var result = await _query.GetAllAsync();

        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Contains(result, t => t.Id == 1L && t.Title == "Test Book 1");
        Assert.Contains(result, t => t.Id == 2L && t.Title == "Test Book 2");
        Assert.Contains(result, t => t.Id == 3L && t.Title == "Test Book 3");
    }
}