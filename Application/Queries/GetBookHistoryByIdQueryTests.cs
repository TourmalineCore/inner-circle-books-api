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

        _context.Books.Add(book);
        _context.BooksCopies.AddRange(bookCopy1, bookCopy2);
        _context.BooksCopiesReadingHistory.AddRange(readingHistory1, readingHistory2);
        await _context.SaveChangesAsync();

        var result = await _query.GetByIdAsync(bookId);

        Assert.NotEmpty(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, x => x.BookCopyId == bookCopy1.Id);
        Assert.Contains(result, x => x.BookCopyId == bookCopy2.Id);
    }
}