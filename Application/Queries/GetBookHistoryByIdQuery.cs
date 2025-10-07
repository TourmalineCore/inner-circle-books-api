using Core;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries;

public class GetBookHistoryByIdQuery
{
    private readonly AppDbContext _context;

    public GetBookHistoryByIdQuery(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(List<BookCopyReadingHistory> List, long TotalCount)> GetByIdAsync(long id, int page, int pageSize)
    {
        var bookCopies = await _context
            .BooksCopies
            .Where(x => x.BookId == id)
            .Select(x => x.Id)
            .ToListAsync();

        var query = _context
            .BooksCopiesReadingHistory
            .Where(x => bookCopies.Contains(x.BookCopyId));

        long totalCount = await query.LongCountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}