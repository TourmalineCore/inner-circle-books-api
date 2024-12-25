using Application.Queries.Contracts;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries
{
    public class GetAllToDosQuery : IGetAllToDosQuery
    {
        private readonly AppDbContext _context;
        public GetAllToDosQuery(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<ToDo>> GetAllAsync()
        {
            var toDoList = await _context.ToDos.ToListAsync();
            return toDoList;
        }
    }
}
