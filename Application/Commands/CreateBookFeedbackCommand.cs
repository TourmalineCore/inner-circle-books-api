using Core;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Commands;

public class CreateBookFeedbackCommandParams
{
  public long BookId { get; set; }

  public ProgressOfReading ProgressOfReading { get; set; }

  public int Rating { get; set; }

  public string? Advantages { get; set; }

  public string? Disadvantages { get; set; }
}

public class CreateBookFeedbackCommand
{
  private readonly AppDbContext _context;

  public CreateBookFeedbackCommand(AppDbContext context)
  {
    _context = context;
  }

  public async Task<long> ExecuteAsync(
    CreateBookFeedbackCommandParams createBookFeedbackCommandParams,
    Employee employee,
    long tenantId
  )
  {
    await _context
      .Books
      .Where(x => x.TenantId == tenantId)
      .Where(x => x.Id == createBookFeedbackCommandParams.BookId)
      .SingleAsync();

    var newBookFeedback = new BookFeedback()
    {
        TenantId = tenantId,
        BookId = createBookFeedbackCommandParams.BookId,
        EmployeeId = employee.Id,
        LeftFeedbackAtUtc = DateTime.UtcNow,
        ProgressOfReading = createBookFeedbackCommandParams.ProgressOfReading,
        Rating = createBookFeedbackCommandParams.Rating,
        Advantages = createBookFeedbackCommandParams.Advantages,
        Disadvantages = createBookFeedbackCommandParams.Disadvantages
    };
    
    await _context
        .BookFeedback
        .AddAsync(newBookFeedback);

    await _context.SaveChangesAsync();

    return newBookFeedback.Id;
  }
}
