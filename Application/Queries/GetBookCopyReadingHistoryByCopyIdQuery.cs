using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries;

public class GetBookCopyReadingHistoryByCopyIdQuery
{
  private readonly AppDbContext _context;

  public GetBookCopyReadingHistoryByCopyIdQuery(AppDbContext context)
  {
    _context = context;
  }

  public Task<BookCopyReadingHistory?> GetActiveReadingAsync(long bookCopyId, long tenantId)
  {
    return _context
      .BooksCopiesReadingHistory
      .Where(x => x.TenantId == tenantId)
      .Where(x => x.BookCopyId == bookCopyId && x.ActualReturnedAtUtc == null)
      .FirstOrDefaultAsync();
  }
}
