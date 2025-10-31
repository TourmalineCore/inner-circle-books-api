using Core;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Commands;

public class ReturnBookCommandParams
{
    public long BookCopyId { get; set; }

    public ProgressOfReading ProgressOfReading { get; set; }

    public DateTime ActualReturnedAtUtc { get; set; }
}

public class ReturnBookCommand
{
    private readonly AppDbContext _context;

    public ReturnBookCommand(AppDbContext context)
    {
        _context = context;
    }

    public async Task ReturnAsync(ReturnBookCommandParams returnBookCommandParams, Employee employee, long tenantId)
    {
        var bookCopyReadingHistory = await _context
            .BooksCopiesReadingHistory
            .Where(x => x.TenantId == tenantId)
            .FirstOrDefaultAsync(x => x.BookCopyId == returnBookCommandParams.BookCopyId
                && x.ReaderEmployeeId == employee.Id
                && x.ActualReturnedAtUtc == null);

        bookCopyReadingHistory.ActualReturnedAtUtc = returnBookCommandParams.ActualReturnedAtUtc;
        bookCopyReadingHistory.ProgressOfReading = returnBookCommandParams.ProgressOfReading;

        _context.BooksCopiesReadingHistory.Update(bookCopyReadingHistory);
        await _context.SaveChangesAsync();
    }
}