using Application;
using Application.Queries;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class GetRecordOfTakingBookCopyByCopyIdQueryTests
{
    private const long TENANT_ID = 1;
    private readonly AppDbContext _context;
    private readonly GetRecordOfTakingBookCopyByCopyIdQuery _query;

    public GetRecordOfTakingBookCopyByCopyIdQueryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("GetRecordOfTakingBookCopyByCopyIdQueryDatabase")
            .Options;

        _context = new AppDbContext(options);
        _query = new GetRecordOfTakingBookCopyByCopyIdQuery(_context);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnRecordOfTakingBookCopy_WhenBookCopyIdExists()
    {
        var bookCopyReadingHistory = new BookCopyReadingHistory
        {
            Id = 1,
            BookCopyId = 1,
            ReaderEmployeeId = 1,
            TakenAtUtc = new DateTime(2025, 11, 22),
            ScheduledReturnDate = new DateOnly(2025, 12, 22),
        };

        await _context.BooksCopiesReadingHistory.AddAsync(bookCopyReadingHistory);
        await _context.SaveChangesAsync();

        var result = await _query.GetAsync(bookCopyReadingHistory.BookCopyId, TENANT_ID);

        Assert.Equal(bookCopyReadingHistory, result);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnNull_WhenRecordOfTakingBookCopyDoesNotExist()
    {
        var nonExistentBookCopyId = 999;

        var result = await _query.GetAsync(nonExistentBookCopyId, TENANT_ID);

        Assert.Null(result);
    }
}