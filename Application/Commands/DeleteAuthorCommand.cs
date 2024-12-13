using Application.Commands.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Application.Commands;

public class DeleteAuthorCommand : IDeleteAuthorCommand
{
    private readonly AppDbContext _context;

    public DeleteAuthorCommand(AppDbContext context)
    {
        _context = context;
    }

    public async Task DeleteAsync(long id, long tenantId)
    {
        var author = await _context.Authors
            .Where(x => x.Id == id && x.TenantId == tenantId)
            .Include(x => x.Books)
            .SingleAsync();

        foreach (var book in author.Books)
        {
            await book.DeleteAuthor(author);
        }

        _context.Authors.Remove(author);

        await _context.SaveChangesAsync();
    }
}