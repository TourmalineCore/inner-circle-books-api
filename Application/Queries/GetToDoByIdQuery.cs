using Application.Queries.Contracts;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries
{
    public class GetToDoByIdQuery : IGetToDoByIdQuery
    {
        private readonly AppDbContext _context;
        public GetToDoByIdQuery(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Book> GetByIdAsync(long id)
        {
            var toDo = await _context.ToDos
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();
            return toDo;
        }
    }
}
