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

    public async Task<List<BookCopyReadingHistory>> GetByIdAsync(long id)
    {
        var bookCopies = await _context
            .BooksCopies
            .Where(x => x.BookId == id)
            .ToListAsync();

        return await _context
            .BooksCopiesReadingHistory
            .Where(h => bookCopies.Select(x => x.Id).Contains(h.BookCopyId))
            .ToListAsync();
    }
}