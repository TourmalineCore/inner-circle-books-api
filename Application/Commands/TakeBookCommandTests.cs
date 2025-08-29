using Core;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Application.Commands;

public class TakeBookCommandTests
{
    private const long TENANT_ID = 1;
    private readonly TakeBookCommand _command;
    private readonly AppDbContext _context;

    public TakeBookCommandTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("TakeBookCommandBooksDatabase")
            .Options;

        _context = new AppDbContext(options);
        _command = new TakeBookCommand(_context);
    }

    [Fact]
    public async Task TakeAsync_ShouldAddNewBookCopyReadingHistoryToDbSet()
    {
        var takeBookRequest = new TakeBookCommandParams
        {
            BookCopyId = 1,
            SсheduledReturnDate = "2025-11-22"
        };

        var employee = new Employee
        {
            EmployeeId = 1,
            FullName = "Ivanov Ivan",
        };

        await _command.TakeAsync(takeBookRequest, employee);

        // Verify the bookCopyReadingHistory is added correctly
        //var bookCopyReadingHistory = await _context.BooksCopiesReadingHistory.FindAsync(bookCopyReadingHistoryId);
        //Assert.NotNull(bookCopyReadingHistory);
        //Assert.Equal(bookCopyReadingHistoryId, bookCopyReadingHistory.Id);
        //Assert.Equal(employee.Id, bookCopyReadingHistory.ReaderEmployeeId);
    }
}