using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries;


public class GetBookFeedbackQuery
{
  private readonly AppDbContext _context;

  public GetBookFeedbackQuery(AppDbContext context)
  {
    _context = context;
  }

  public Task<List<BookFeedback>> GetAsync(long bookId, long tenantId)
  {
    return _context
      .BookFeedback
      .AsNoTracking()
      .Where(x => x.TenantId == tenantId)
      .Where(x => x.BookId == bookId)
      .Where(x => x.Book.DeletedAtUtc == null)
      .ToListAsync();
  }
}
