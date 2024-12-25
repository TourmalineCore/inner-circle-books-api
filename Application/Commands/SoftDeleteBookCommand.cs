using Application.Commands.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Application.Commands;

public class SoftDeleteBookCommand : ISoftDeleteBookCommand
{
    private readonly AppDbContext _context;

    public SoftDeleteBookCommand(AppDbContext context)
    {
        _context = context;
    }

    public async Task SoftDeleteAsync(long id, long tenantId)
    {
        var book = await _context.Books
            .Where(x => x.Id == id && x.TenantId == tenantId)
            .SingleOrDefaultAsync();

        if (book != null)
        {
            book.DeletedAtUtc = DateTime.UtcNow;

            _context.Books.Update(book);
            await _context.SaveChangesAsync();
        }
    }
}