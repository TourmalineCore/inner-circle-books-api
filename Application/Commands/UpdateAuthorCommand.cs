using Application.Commands.Contracts;
using Application.Requests;
using Microsoft.EntityFrameworkCore;

namespace Application.Commands;

public class UpdateAuthorCommand : IUpdateAuthorCommand
{
    private readonly AppDbContext _context;

    public UpdateAuthorCommand(AppDbContext context)
    {
        _context = context;
    }

    public async Task UpdateAsync(long id, UpdateAuthorRequest updateAuthorRequest, long tenantId)
    {
        var author = await _context.Authors
            .Where(x => x.Id == id && x.TenantId == tenantId)
            .SingleAsync();

        author.FullName = updateAuthorRequest.FullName;

        _context.Authors.Update(author);
        await _context.SaveChangesAsync();
    }
}