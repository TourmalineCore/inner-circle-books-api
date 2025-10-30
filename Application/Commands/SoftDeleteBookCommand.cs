using Microsoft.EntityFrameworkCore;

namespace Application.Commands;

public class SoftDeleteBookCommand
{
    private readonly AppDbContext _context;

    public SoftDeleteBookCommand(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> SoftDeleteAsync(long id, long tenantId)
    {
        var book = await _context.Books
            .Where(x => x.TenantId == tenantId)
            .Where(x => x.Id == id)
            .SingleOrDefaultAsync();

        if (book == null)
        {
            return false;
        }

        book.DeletedAtUtc = DateTime.UtcNow;

        _context.Books.Update(book);
        await _context.SaveChangesAsync();

        return true;
    }
}