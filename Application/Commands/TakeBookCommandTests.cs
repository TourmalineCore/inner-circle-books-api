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
    private readonly ReturnBookCommand _returnBookCommand;
    private readonly IInnerCircleHttpClient _client;

    public TakeBookCommandTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("TakeBookCommandBooksDatabase")
            .Options;

        _context = new AppDbContext(options);
        _client = new MockInnerCircleHttpClient();

        _returnBookCommand = new ReturnBookCommand(_context);
        _command = new TakeBookCommand(_context, _client, _returnBookCommand);
    }

    [Fact]
    public async Task TakeAsync_ShouldAddNewBookCopyReadingHistoryToDbSet()
    {
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

    [Fact]
    public async Task TakeAsync_WhenBookIsAlreadyTaken_ShouldReturnPreviousAndAddNew()
    {
        var existingReader = new Employee { Id = 2, FullName = "Previous Reader" };
        _context.BooksCopiesReadingHistory.Add(new BookCopyReadingHistory
        {
            BookCopyId = 2,
            ReaderEmployeeId = existingReader.Id,
            TakenAtUtc = DateTime.UtcNow.AddDays(-10),
            ScheduledReturnDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
            ActualReturnedAtUtc = null
        });
        await _context.SaveChangesAsync();

        var newEmployee = new Employee { Id = 3, FullName = "New Reader" };

        var request = new TakeBookCommandParams
        {
            BookCopyId = 2,
            ScheduledReturnDate = "2025-12-10"
        };

        await _command.TakeAsync(request, newEmployee);

        var oldRecord = await _context.BooksCopiesReadingHistory
            .FirstAsync(x => x.ReaderEmployeeId == existingReader.Id && x.BookCopyId == 2);

        Assert.NotNull(oldRecord.ActualReturnedAtUtc);

        var newRecord = await _context.BooksCopiesReadingHistory
            .FirstOrDefaultAsync(x => x.ReaderEmployeeId == newEmployee.Id && x.BookCopyId == 2);

        Assert.NotNull(newRecord);
        Assert.Equal(new DateOnly(2025, 12, 10), newRecord.ScheduledReturnDate);
        Assert.Null(newRecord.ActualReturnedAtUtc);
    }
}

public class MockInnerCircleHttpClient : IInnerCircleHttpClient
{
    public Task<Employee> GetEmployeeAsync(string corporateEmail)
    {
        return Task.FromResult(new Employee
        {
            Id = 999,
            FullName = "Fake Employee",
            CorporateEmail = corporateEmail
        });
    }

    public Task<List<EmployeeById>> GetEmployeesByIdsAsync(List<long> ids)
    {
        var employees = ids.Select(id => new EmployeeById
        {
            EmployeeId = id,
            FullName = $"Employee {id}"
        }).ToList();

        return Task.FromResult(employees);
    }
}
