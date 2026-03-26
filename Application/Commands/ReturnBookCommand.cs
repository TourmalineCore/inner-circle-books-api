using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Commands;

public class ReturnBookCommandParams
{
  public long BookCopyId { get; set; }

  public long BookId { get; set; }

  public long EmployeeId { get; set; }

  public ProgressOfReading ProgressOfReading { get; set; }

  public DateTime ActualReturnedAtUtc { get; set; }

  public int Rating { get; set; }

  public string? Advantages { get; set; }

  public string? Disadvantages { get; set; }
}

public class ReturnBookCommand
{
  private readonly AppDbContext _context;

  public ReturnBookCommand(AppDbContext context)
  {
    _context = context;
  }

  public async Task ReturnAsync(
    ReturnBookCommandParams returnBookCommandParams,
    long tenantId
  )
  {
    var bookCopyReadingHistory = await _context
      .BooksCopiesReadingHistory
      .Where(x => x.TenantId == tenantId)
      .FirstOrDefaultAsync(x => x.BookCopyId == returnBookCommandParams.BookCopyId
        && x.ReaderEmployeeId == returnBookCommandParams.EmployeeId
        && x.ActualReturnedAtUtc == null);

    bookCopyReadingHistory.ActualReturnedAtUtc = returnBookCommandParams.ActualReturnedAtUtc;
    bookCopyReadingHistory.ProgressOfReading = returnBookCommandParams.ProgressOfReading;

    _context.BooksCopiesReadingHistory.Update(bookCopyReadingHistory);

    var bookFeedback = new BookFeedback
    {
      BookId = returnBookCommandParams.BookId,
      EmployeeId = returnBookCommandParams.EmployeeId,
      LeftFeedbackAtUtc = DateTime.UtcNow,
      ProgressOfReading = returnBookCommandParams.ProgressOfReading,
      Rating = returnBookCommandParams.Rating,
      Advantages = returnBookCommandParams.Advantages,
      Disadvantages = returnBookCommandParams.Disadvantages
    };

    await _context.BookFeedback.AddAsync(bookFeedback);

    await _context.SaveChangesAsync();
  }
}
