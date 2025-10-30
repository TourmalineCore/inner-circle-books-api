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

    public TakeBookCommand(AppDbContext context)
    {
        _context = context;
    }

    public async Task<long> TakeAsync(TakeBookCommandParams takeBookCommandParams, Employee employee, long tenantId)
    {
        var bookCopyExists = await _context.BooksCopies
            .AnyAsync(x => x.Id == takeBookCommandParams.BookCopyId);

        if (!bookCopyExists)
        {
            throw new ArgumentException($"BookCopy with id {takeBookCommandParams.BookCopyId} does not exist");
        }

        var result = DateTime.Parse(takeBookCommandParams.ScheduledReturnDate, null, System.Globalization.DateTimeStyles.RoundtripKind);

        var bookCopyReadingHistory = new BookCopyReadingHistory
        {
            BookCopyId = takeBookCommandParams.BookCopyId,
            ReaderEmployeeId = employee.Id,
            TakenAtUtc = DateTime.UtcNow,
            ScheduledReturnDate = new DateOnly(result.Year, result.Month, result.Day),
            TenantId = tenantId
        };

        await _context.BooksCopiesReadingHistory.AddAsync(bookCopyReadingHistory);
        await _context.SaveChangesAsync();
    }
}