using Microsoft.EntityFrameworkCore;

namespace Application.Queries;

public class GetCopiesIdsByBookIdQuery
{
    private readonly AppDbContext _context;

    public GetCopiesIdsByBookIdQuery(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<long>> GetByBookIdAsync(long id)
    {
        return await _context.BooksCopies
            .Where(x => x.BookId == id)
            .Select(x => x.Id)
            .ToListAsync();
    }
}
