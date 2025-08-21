using Core;
using Core.Entities;
using NodaTime;

namespace Application.Commands;

public class TakeBookCommandParams
{
    public long BookCopyId { get; set; }

    public string SheduledReturnDate { get; set; }
}

public class TakeBookCommand
{
    private readonly AppDbContext _context;

    public TakeBookCommand(AppDbContext context)
    {
        _context = context;
    }

    public async Task<long> TakeAsync(TakeBookCommandParams takeBookCommandParams, Employee employee)
    {
        var result = DateTime.Parse(takeBookCommandParams.SheduledReturnDate, null, System.Globalization.DateTimeStyles.RoundtripKind);

        var bookCopyReadingHistory = new BookCopyReadingHistory
        {
            BookCopyId = takeBookCommandParams.BookCopyId,
            ReaderEmployeeId = employee.Id,
            TakenAtUtc = DateTime.UtcNow,
            SheduledReturnDate = new DateOnly(result.Year, result.Month, result.Day),
        };

        await _context.BooksCopiesReadingHistory.AddAsync(bookCopyReadingHistory);
        await _context.SaveChangesAsync();

        return bookCopyReadingHistory.Id;
    }
}