using Application.Queries.Contracts;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries
{
    public class GetBookByIdQuery : IGetBookByIdQuery
    {
        private readonly AppDbContext _context;
        public GetBookByIdQuery(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Book> GetByIdAsync(long id)
        {
            var toDo = await _context.Books
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();
            return toDo;
        }
    }
}