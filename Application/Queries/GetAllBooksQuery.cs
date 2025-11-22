using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries;

public class GetAllBooksQuery
{
  private readonly AppDbContext _context;

  public GetAllBooksQuery(AppDbContext context)
  {
    _context = context;
  }

  public async Task<List<Book>> GetAllAsync(long tenantId)
  {
    var booksList = await _context
      .Books
      .Where(x => x.TenantId == tenantId)
      .Where(x => x.DeletedAtUtc == null)
      .ToListAsync();

    return booksList;
  }
}
