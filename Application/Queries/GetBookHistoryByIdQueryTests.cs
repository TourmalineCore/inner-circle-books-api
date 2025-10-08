using Application;
using Application.Queries;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class GetBookHistoryByIdQueryTests
{
    private readonly AppDbContext _context;
    private readonly GetBookHistoryByIdQuery _query;

    public GetBookHistoryByIdQueryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("GetBookHistoryByIdQueryBooksDatabase")
            .Options;

        _context = new AppDbContext(options);
        _query = new GetBookHistoryByIdQuery(_context);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnReadingHistory()
    {
        var bookId = 1L;

        var book = new Book
        {
            Id = 1,
            TenantId = 1,
            Title = "Test Book 1",
            Annotation = "Test annotation 1",
            Authors = new List<Author>()
            {
                new Author()
                {
                    FullName = "Test Author"
                }
            },
            Language = Language.en,
            CoverUrl = ""
        };

        var bookCopy1 = new BookCopy
        {
            Id = 3L,
            BookId = bookId
        };
        var bookCopy2 = new BookCopy
        {
            Id = 4L,
            BookId = bookId
        };
        var bookCopy3 = new BookCopy
        {
            Id = 5L,
            BookId = bookId
        };

        var readingHistory1 = new BookCopyReadingHistory
        {
            Id = 1L,
            BookCopyId = bookCopy1.Id,
            ReaderEmployeeId = 1L,
            TakenAtUtc = new DateTime(2025, 1, 1),
            ScheduledReturnDate = new DateOnly(2025, 2, 1)
        };
        var readingHistory2 = new BookCopyReadingHistory
        {
            Id = 2L,
            BookCopyId = bookCopy2.Id,
            ReaderEmployeeId = 2L,
            TakenAtUtc = new DateTime(2025, 3, 1),
            ScheduledReturnDate = new DateOnly(2025, 4, 1)
        };
        var readingHistory3 = new BookCopyReadingHistory
        {
            Id = 3L,
            BookCopyId = bookCopy3.Id,
            ReaderEmployeeId = 3L,
            TakenAtUtc = new DateTime(2025, 5, 1),
            ScheduledReturnDate = new DateOnly(2025, 6, 1)
        };

        _context.Books.Add(book);
        _context.BooksCopies.AddRange(bookCopy1, bookCopy2, bookCopy3);
        _context.BooksCopiesReadingHistory.AddRange(readingHistory1, readingHistory2, readingHistory3);
        await _context.SaveChangesAsync();

        var (items, totalCount) = await _query.GetByIdAsync(bookId, page: 1, pageSize: 2);

        Assert.NotEmpty(items);
        Assert.Equal(2, items.Count);
        Assert.Equal(3, totalCount);
        Assert.Contains(items, x => x.BookCopyId == bookCopy3.Id);
        Assert.Contains(items, x => x.BookCopyId == bookCopy2.Id);
        Assert.True(items[0].TakenAtUtc > items[1].TakenAtUtc);
    }
}