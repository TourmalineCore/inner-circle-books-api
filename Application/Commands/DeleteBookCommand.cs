using Microsoft.EntityFrameworkCore;

namespace Application.Commands;

public class DeleteBookCommand
{
    private readonly AppDbContext _context;

    public DeleteBookCommand(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> DeleteAsync(long id, long tenantId)
    {
        var book = await _context.Books
            .Where(x => x.TenantId == tenantId)
            .Where(x => x.Id == id)
            .SingleOrDefaultAsync();

        if (book == null)
        {
            return false;
        }

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();

        return true;
    }
}