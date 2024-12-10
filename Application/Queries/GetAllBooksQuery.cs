using Application.Queries.Contracts;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries
{
    public class GetAllBooksQuery : IGetAllBooksQuery
    {
        private readonly AppDbContext _context;
        public GetAllBooksQuery(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Book>> GetAllAsync()
        {
            var toDoList = await _context.Books
                .Include(x => x.Authors)
                .ToListAsync();
            return toDoList;
        }
    }
}