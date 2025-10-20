using Core;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Commands;

public class TakeBookCommandParams
{
    public long BookCopyId { get; set; }

    public string ScheduledReturnDate { get; set; }
}

public class TakeBookCommand
{
    private readonly AppDbContext _context;

    private readonly ReturnBookCommand _returnBookCommand;

    private readonly IInnerCircleHttpClient _client;

    public TakeBookCommand(AppDbContext context, IInnerCircleHttpClient client, ReturnBookCommand returnBookCommand)
    {
        _context = context;
        _client = client;
        _returnBookCommand = returnBookCommand;
    }

    public async Task TakeAsync(TakeBookCommandParams takeBookCommandParams, Employee employee)
    {
        var result = DateTime.Parse(takeBookCommandParams.ScheduledReturnDate, null, System.Globalization.DateTimeStyles.RoundtripKind);

        var readerEmployeeId = await _context.BooksCopiesReadingHistory
                .Where(x => x.BookCopyId == takeBookCommandParams.BookCopyId
                         && x.ActualReturnedAtUtc == null)
                .Select(x => x.ReaderEmployeeId)
                .FirstOrDefaultAsync();

        if (readerEmployeeId != default(long))
        {
            var employeesByIds = await _client.GetEmployeesByIdsAsync(new List<long> { readerEmployeeId });

            var currentReader = new Employee
            {
                Id = employeesByIds[0].EmployeeId,
                FullName = employeesByIds[0].FullName,
                CorporateEmail = null,
                TenantId = 0
            };

            var returnBookCommandParams = new ReturnBookCommandParams
            {
                BookCopyId = takeBookCommandParams.BookCopyId,
                ProgressOfReading = ProgressOfReading.NotSpecified,
                ActualReturnedAtUtc = DateTime.UtcNow
            };

            await _returnBookCommand.ReturnAsync(returnBookCommandParams, currentReader);
        }

        var bookCopyReadingHistory = new BookCopyReadingHistory
        {
            BookCopyId = takeBookCommandParams.BookCopyId,
            ReaderEmployeeId = employee.Id,
            TakenAtUtc = DateTime.UtcNow,
            ScheduledReturnDate = new DateOnly(result.Year, result.Month, result.Day),
        };

        await _context.BooksCopiesReadingHistory.AddAsync(bookCopyReadingHistory);
        await _context.SaveChangesAsync();
    }
}