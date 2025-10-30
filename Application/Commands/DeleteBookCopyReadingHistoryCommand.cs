using Microsoft.EntityFrameworkCore;

namespace Application.Commands;

public class DeleteBookCopyReadingHistoryCommand
{
    private readonly AppDbContext _context;

    public DeleteBookCopyReadingHistoryCommand(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> DeleteAsync(long id, long tenantId)
    {
        var bookCopyReadingHistory = await _context.BooksCopiesReadingHistory
            .Where(x => x.TenantId == tenantId)
            .Where(x => x.Id == id)
            .SingleOrDefaultAsync();

        if (bookCopyReadingHistory == null)
        {
            return false;
        }

        _context.BooksCopiesReadingHistory.Remove(bookCopyReadingHistory);
        await _context.SaveChangesAsync();

        return true;
    }
}