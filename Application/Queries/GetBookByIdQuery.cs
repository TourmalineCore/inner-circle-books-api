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

    public async Task<Book> GetByIdAsync(long id, long tenantId)
    {
        var book = await _context.Books
            .Where(x => x.TenantId == tenantId)
            .Where(x => x.Id == id)
            .SingleOrDefaultAsync();
        return book;
    }

    public GetBookByIdQuery()
    {
    }
}