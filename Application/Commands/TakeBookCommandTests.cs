using Core;
using Core.Entities;
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
        var book = new Book
        {
            Id = 1,
            Title = "Some test book",
            Annotation = "Test annotation",
            TenantId = TENANT_ID,
            CreatedAtUtc = DateTime.UtcNow,
            Language = Language.en,
            Authors = new List<Author>()
        };

        _context.Books.Add(book);

        await _context.SaveChangesAsync();

        var bookCopy = new BookCopy { Id = 1, BookId = book.Id };

        _context.BooksCopies.Add(bookCopy);

        await _context.SaveChangesAsync();

        var takeBookRequest = new TakeBookCommandParams
        {
            BookCopyId = 1,
            ScheduledReturnDate = "2025-11-22"
        };

        var employee = new Employee
        {
            Id = 1,
            FullName = "Ivanov Ivan",
        };

        await _command.TakeAsync(takeBookRequest, employee);

        var bookCopyReadingHistory = await _context
            .BooksCopiesReadingHistory
            .FirstOrDefaultAsync(x => x.BookCopyId == takeBookRequest.BookCopyId && x.ReaderEmployeeId == employee.Id);

        Assert.NotNull(bookCopyReadingHistory);
        Assert.Equal(takeBookRequest.BookCopyId, bookCopyReadingHistory.BookCopyId);
        Assert.Equal(employee.Id, bookCopyReadingHistory.ReaderEmployeeId);
        Assert.Equal(new DateOnly(2025, 11, 22), bookCopyReadingHistory.ScheduledReturnDate);
    }
}
