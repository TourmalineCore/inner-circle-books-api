using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries;

public class GetBookByCopyIdQuery
{
  private readonly AppDbContext _context;

  public GetBookByCopyIdQuery(AppDbContext context)
  {
    _context = context;
  }

  public Task<Book?> GetByCopyIdAsync(long copyId, long tenantId)
  {
    return _context
      .BooksCopies
      .Where(x => x.TenantId == tenantId)
      .Where(x => x.Id == copyId)
      .Select(x => x.Book)
      .SingleOrDefaultAsync();
  }
}
