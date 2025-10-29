using Microsoft.EntityFrameworkCore;

namespace Application.Commands;

public class DeleteBookCopyCommand
{
    private readonly AppDbContext _context;

    public DeleteBookCopyCommand(AppDbContext context)
    {
        _context = context;
    }

    public async Task DeleteAsync(long id, long tenantId)
    {
        var bookCopy = await _context.BooksCopies
            .Where(x => x.TenantId == tenantId)
            .Where(x => x.Id == id)
            .SingleOrDefaultAsync();

        if (bookCopy != null)
        {
            _context.BooksCopies.Remove(bookCopy);
            await _context.SaveChangesAsync();
        }
    }
}