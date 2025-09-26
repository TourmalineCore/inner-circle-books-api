using Core;
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

    public Task<long> GetBookIdByCopyIdAsync(long copyId)
    {
        return _context
            .BooksCopies
            .Where(x => x.Id == copyId)
            .Select(x => x.BookId)
            .FirstOrDefaultAsync();
    }
}