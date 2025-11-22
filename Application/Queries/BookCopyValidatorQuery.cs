using Microsoft.EntityFrameworkCore;

namespace Application.Queries;

public class BookCopyValidatorQuery
{
  private readonly AppDbContext _context;

  public BookCopyValidatorQuery(AppDbContext context)
  {
    _context = context;
  }

  public Task<bool> IsValidSecretKeyAsync(long bookCopyId, string secretKey, long tenantId)
  {
    return _context
      .BooksCopies
      .Where(x => x.TenantId == tenantId)
      .AnyAsync(x => x.Id == bookCopyId && x.SecretKey == secretKey);
  }
}
