using Application;
using Application.Queries;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class GetBookCopyReadingHistoryByCopyIdQueryTests
{
  private const long TENANT_ID = 1;
  private readonly AppDbContext _context;
  private readonly GetBookCopyReadingHistoryByCopyIdQuery _query;

  public GetBookCopyReadingHistoryByCopyIdQueryTests()
  {
    var options = new DbContextOptionsBuilder<AppDbContext>()
      .UseInMemoryDatabase("GetBookCopyReadingHistoryByCopyIdQueryDatabase")
      .Options;

    _context = new AppDbContext(options);
    _query = new GetBookCopyReadingHistoryByCopyIdQuery(_context);
  }

  [Fact]
  public async Task GetAsync_ShouldReturnBookCopyReadingHistoryRecord_WhenBookCopyReadingHistoryExistsByBookCopyId()
  {
    var bookCopyReadingHistory = new BookCopyReadingHistory
    {
      Id = 1,
      BookCopyId = 1,
      ReaderEmployeeId = 1,
      TakenAtUtc = new DateTime(2025, 11, 22),
      ScheduledReturnDate = new DateOnly(2025, 12, 22),
      TenantId = TENANT_ID
    };

    await _context.BooksCopiesReadingHistory.AddAsync(bookCopyReadingHistory);
    await _context.SaveChangesAsync();

    var result = await _query.GetActiveReadingAsync(bookCopyReadingHistory.BookCopyId, TENANT_ID);

    Assert.Equal(bookCopyReadingHistory, result);
  }

  [Fact]
  public async Task GetAsync_ShouldReturnNull_WhenBookCopyReadingHistoryNotExistByBookCopyId()
  {
    var nonExistentBookCopyId = 999;

    var result = await _query.GetActiveReadingAsync(nonExistentBookCopyId, TENANT_ID);

    Assert.Null(result);
  }
}
