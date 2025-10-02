using Application;
using Application.Commands;
using Core;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class ReturnBookCommandTests
{
    private readonly ReturnBookCommand _command;
    private readonly AppDbContext _context;

    public ReturnBookCommandTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("ReturnBookCommandBooksDatabase")
            .Options;

        _context = new AppDbContext(options);
        _command = new ReturnBookCommand(_context);
    }

    [Fact]
    public async Task ReturnAsync_ShouldCompleteBookCopyReadingHistory()
    {
        var dateTimeUtcPlusTwoMonths = DateTime.UtcNow.AddMonths(2);

        var bookCopyReadingHistory = new BookCopyReadingHistory
        {
            Id = 1,
            BookCopyId = 1,
            ReaderEmployeeId = 1,
            TakenAtUtc = DateTime.UtcNow,
            ScheduledReturnDate = DateOnly.FromDateTime(dateTimeUtcPlusTwoMonths)
        };

        _context.BooksCopiesReadingHistory.Add(bookCopyReadingHistory);
        await _context.SaveChangesAsync();

        var returnBookRequest = new ReturnBookCommandParams
        {
            BookCopyId = 1,
            ProgressOfReading = 0,
            ActualReturnedAtUtc = DateTime.UtcNow
        };

        var employee = new Employee
        {
            Id = 1
        };

        await _command.ReturnAsync(returnBookRequest, employee);

        var completedCopyReadingHistory = await _context.BooksCopiesReadingHistory.FindAsync(bookCopyReadingHistory.Id);
        Assert.NotNull(completedCopyReadingHistory);
        Assert.Equal(returnBookRequest.ActualReturnedAtUtc, completedCopyReadingHistory.ActualReturnedAtUtc);
        Assert.Equal(returnBookRequest.ProgressOfReading, completedCopyReadingHistory.ProgressOfReading);
    }
}