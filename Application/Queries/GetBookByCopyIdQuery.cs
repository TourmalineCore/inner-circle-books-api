using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries;

public interface IGetBookByCopyIdQuery
{
  Task<Book?> GetByCopyIdAsync(long copyId, long tenantId);
}

public class GetBookByCopyIdQuery : IGetBookByCopyIdQuery
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
