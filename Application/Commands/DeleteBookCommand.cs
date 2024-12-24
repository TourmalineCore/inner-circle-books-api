using Application.Commands.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Application.Commands;

public class DeleteBookCommand : IDeleteBookCommand
{
    private readonly AppDbContext _context;

    public DeleteBookCommand(AppDbContext context)
    {
        _context = context;
    }

    public async Task DeleteAsync(long id, long tenantId)
    {
        var book = await _context.Books
            .Where(x => x.Id == id && x.TenantId == tenantId)
            .SingleOrDefaultAsync();

        if (book != null)
        {
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
        }
    }
}