using Core;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries;

public class GetBookByIdQuery
{
    private readonly AppDbContext _context;

    public GetBookByIdQuery(AppDbContext context)
    {
        _context = context;
    }

    public Task<Book> GetByIdAsync(long id, long tenantId)
    {
        return _context
            .Books
            .Where(x => x.TenantId == tenantId)
            .Include(x => x.Copies)
            .SingleAsync(x => x.Id == id);
    }

    public Task<List<EmployeeWhoReadsNow>> GetEmployeesWhoReadNowAsync(List<long> copiesIds)
    {
        return _context
            .BooksCopiesReadingHistory
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