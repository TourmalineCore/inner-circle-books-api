using Application.Commands.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Application.Commands
{
    public class DeleteBookCommand : IDeleteToDoCommand
    {
        private readonly AppDbContext _context;

        public DeleteBookCommand(AppDbContext context)
        {
            _context = context;
        }

        public async Task DeleteAsync(long id)
        {
            var book = await _context.Books
                .Where(x => x.Id == id)
                .SingleAsync();
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
        }
    }
}
