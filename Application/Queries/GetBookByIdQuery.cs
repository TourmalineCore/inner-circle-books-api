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

    public Task<List<long>> GetEmployeesIdsByCopiesIdsAsync(List<long> copiesIds)
    {
        return _context
            .BooksCopiesReadingHistory
            .Where(x => copiesIds.Contains(x.BookCopyId))
            .Where(x => x.ActualReturnedAtUtc == null)
            .Select(x => x.ReaderEmployeeId)
            .ToListAsync();
    }

    public async Task<List<EmployeeWhoReadsNow>> GetEmployeesWhoReadNowAsync(List<EmployeeById> employeesByIds)
    {
        var employeesIds = employeesByIds
            .Select(x => x.EmployeeId)
            .ToList();

        var readingHistory = await _context
            .BooksCopiesReadingHistory
            .Where(x => employeesIds.Contains(x.ReaderEmployeeId))
            .ToListAsync();

        return employeesByIds
            .Select(employee =>
            {
                var bookCopy = readingHistory.FirstOrDefault(x => x.ReaderEmployeeId == employee.EmployeeId);

                return new EmployeeWhoReadsNow
                {
                    EmployeeId = employee.EmployeeId,
                    FullName = employee.FullName,
                    BookCopyId = bookCopy.BookCopyId
                };
            })
            .ToList();
    }
}