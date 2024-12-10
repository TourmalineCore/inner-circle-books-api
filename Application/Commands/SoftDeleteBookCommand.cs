using Application.Commands.Contracts;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace Application.Commands
{
    public class SoftDeleteBookCommand : ISoftDeleteBookCommand
    {
        private readonly AppDbContext _context;

        public SoftDeleteBookCommand(AppDbContext context)
        {
            _context = context;
        }

        public async Task SoftDeleteAsync(long id)
        {
            var book = await _context.Books
                .Where(x => x.Id == id)
                .SingleAsync();
            book.DeletedAtUtc = DateTime.UtcNow;
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
        }
    }
}