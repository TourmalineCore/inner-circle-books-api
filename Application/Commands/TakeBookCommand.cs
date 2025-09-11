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

    public async Task TakeAsync(TakeBookCommandParams takeBookCommandParams, Employee employee)
    {
        var result = DateTime.Parse(takeBookCommandParams.ScheduledReturnDate, null, System.Globalization.DateTimeStyles.RoundtripKind);

        var isAlreadyTaken = await _context.BooksCopiesReadingHistory
                .AnyAsync(x => x.BookCopyId == takeBookCommandParams.BookCopyId 
                            && x.ActualReturnedAtUtc == null);

        if (isAlreadyTaken)
        {
            throw new InvalidOperationException("This copy of the book is already taken by another employee.");
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