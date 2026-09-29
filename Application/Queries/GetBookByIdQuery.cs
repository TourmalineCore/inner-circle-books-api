using Core;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries;

public interface IGetBookByIdQuery
{
  Task<Book?> GetByIdAsync(long bookId, long tenantId);
  Task<List<EmployeeWhoReadsNow>> GetEmployeesWhoReadNowAsync(List<long> copiesIds, long tenantId);
}

public class GetBookByIdQuery: IGetBookByIdQuery
{
  private readonly AppDbContext _context;

  public GetBookByIdQuery(AppDbContext context)
  {
    _context = context;
  }

  public Task<Book?> GetByIdAsync(long bookId, long tenantId)
  {
    return _context
      .Books
      .AsNoTracking()
      .Where(x => x.TenantId == tenantId)
      .Where(x => x.DeletedAtUtc == null)
      .Include(x => x.Copies)
      .Include(x => x.KnowledgeAreas) 
      .SingleOrDefaultAsync(x => x.Id == bookId);
  }

  public Task<List<EmployeeWhoReadsNow>> GetEmployeesWhoReadNowAsync(List<long> copiesIds, long tenantId)
  {
    return _context
      .BooksCopiesReadingHistory
      .AsNoTracking()
      .Where(x => x.TenantId == tenantId)
      .Where(x => copiesIds.Contains(x.BookCopyId))
      .Where(x => x.ActualReturnedAtUtc == null)
      .Select(x => new EmployeeWhoReadsNow
      {
        EmployeeId = x.ReaderEmployeeId,
        BookCopyId = x.BookCopyId,
      }
      )
      .ToListAsync();
  }
}
