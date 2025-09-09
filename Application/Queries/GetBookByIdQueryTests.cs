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
    public async Task GetByIdAsync_ShouldReturnNull_WhenBookDoesNotExist()
    {
        var nonExistentId = 999;

        await Assert.ThrowsAsync<InvalidOperationException>(() => _query.GetByIdAsync(nonExistentId, TENANT_ID));
    }

    private void GetEmployeesIdsByCopiesIdsAsync_SeedTestData()
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
            CoverUrl = "http://test1-images.com/img404.png",
            Copies = new List<BookCopy>()
        };

        _context.Books.AddRange(book);

        var copy1 = new BookCopy
        {
            Id = 1,
            BookId = 1,
            Book = book,
            ReadingHistoryList = new List<BookCopyReadingHistory>()
        };

        var copy2 = new BookCopy
        {
            Id = 2,
            BookId = 1,
            Book = book,
            ReadingHistoryList = new List<BookCopyReadingHistory>()
        };

        _context.BooksCopies.AddRange(copy1, copy2);

        book.Copies.Add(copy1);
        book.Copies.Add(copy2);

        var readingHistory1 = new BookCopyReadingHistory
        {
            Id = 1,
            BookCopyId = 1,
            BookCopy = copy1,
            ReaderEmployeeId = 101,
            TakenAtUtc = DateTime.UtcNow.AddDays(-10),
            SсheduledReturnDate = new DateOnly(2023, 12, 25),
            ActualReturnedAtUtc = DateTime.UtcNow.AddDays(-5)
        };

        var readingHistory2 = new BookCopyReadingHistory
        {
            Id = 2,
            BookCopyId = 2,
            BookCopy = copy2,
            ReaderEmployeeId = 102,
            TakenAtUtc = DateTime.UtcNow.AddDays(-8),
            SсheduledReturnDate = new DateOnly(2023, 12, 27),
            ActualReturnedAtUtc = DateTime.UtcNow.AddDays(-3)
        };

        var readingHistory3 = new BookCopyReadingHistory
        {
            Id = 3,
            BookCopyId = 1,
            BookCopy = copy1,
            ReaderEmployeeId = 102,
            TakenAtUtc = DateTime.UtcNow.AddDays(-8),
            SсheduledReturnDate = new DateOnly(2023, 12, 27),
            ActualReturnedAtUtc = DateTime.UtcNow.AddDays(-3)
        };

        _context.BooksCopiesReadingHistory.AddRange(readingHistory1, readingHistory2, readingHistory3);

        copy1.ReadingHistoryList.Add(readingHistory1);
        copy2.ReadingHistoryList.Add(readingHistory2);
        copy1.ReadingHistoryList.Add(readingHistory3);

        _context.SaveChanges();
    }

    [Fact]
    public async Task GetEmployeesIdsByCopiesIdsAsync_WithCopy1_ReturnsCorrectReaderIds()
    {
        var copiesIds = new List<long> { 1 };

        var result = await _query.GetEmployeesIdsByCopiesIdsAsync(copiesIds);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(101, result);
        Assert.Contains(102, result);
    }

    [Fact]
    public async Task GetEmployeesIdsByCopiesIdsAsync_WithCopies1And2_ReturnsCorrectReaderIds()
    {
        var copiesIds = new List<long> { 1, 2 };

        var result = await _query.GetEmployeesIdsByCopiesIdsAsync(copiesIds);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(101, result);
        Assert.Contains(102, result);
    }

    [Fact]
    public async Task GetEmployeesIdsByCopiesIdsAsync_WithNonExistentCopies_ReturnsEmptyList()
    {
        var copiesIds = new List<long> { 999 };

        var result = await _query.GetEmployeesIdsByCopiesIdsAsync(copiesIds);

        Assert.Empty(result);
    }
}