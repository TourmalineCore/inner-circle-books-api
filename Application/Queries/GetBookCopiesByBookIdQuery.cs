using System;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries;

public class GetBookCopiesByBookIdQuery
{
    private readonly AppDbContext _context;

    public GetBookCopiesByBookIdQuery(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<BookCopy>> GetByBookIdAsync(long id)
    {
        var bookCopies = await _context.BooksCopies
            .Where(x => x.BookId == id)
            .ToListAsync();

        return bookCopies;
    }
}
