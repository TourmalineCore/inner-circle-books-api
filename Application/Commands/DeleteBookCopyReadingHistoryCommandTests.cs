using Application;
using Application.Commands;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class DeleteBookCopyReadingHistoryCommandTests
{
    private const long TENANT_ID = 1;
    private readonly DeleteBookCopyReadingHistoryCommand _command;
    private readonly AppDbContext _context;

    public DeleteBookCopyReadingHistoryCommandTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("DeleteBookCopyReadingHistoryCommandBooksDatabase")
            .Options;

        _context = new AppDbContext(options);
        _command = new DeleteBookCopyReadingHistoryCommand(_context);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteBookCopyReadingHistoryFromDbSet()
    {
        var bookCopyReadingHistory = new BookCopyReadingHistory
        {
            Id = 1,
            BookCopyId = 1,
            ReaderEmployeeId = 1,
            TakenAtUtc = new DateTime(2025, 1, 1),
            ScheduledReturnDate = new DateOnly(2025, 2, 1),
            TenantId = TENANT_ID
        };

        _context.BooksCopiesReadingHistory.Add(bookCopyReadingHistory);
        await _context.SaveChangesAsync();

        await _command.DeleteAsync(bookCopyReadingHistory.Id, bookCopyReadingHistory.TenantId);

        var isBookCopyReadingHistoryExistInDbSet = await _context.BooksCopiesReadingHistory
            .Where(x => x.TenantId == TENANT_ID)
            .AnyAsync(x => x.Id == bookCopyReadingHistory.Id);

        Assert.False(isBookCopyReadingHistoryExistInDbSet);
    }
}