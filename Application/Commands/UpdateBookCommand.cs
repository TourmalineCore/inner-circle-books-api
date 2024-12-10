using Application.Commands.Contracts;
using Application.Requests;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace Application.Commands
{
    public class UpdateBookCommand : IUpdateBookCommand
    {
        private readonly AppDbContext _context;

        public UpdateBookCommand(AppDbContext context)
        {
            _context = context;
        }

        public async Task UpdateAsync(UpdateBookRequest updateBookRequest)
        {
            var book = await _context.Books
                .Where(x => x.Id == updateBookRequest.Id)
                .SingleAsync();

            book.Title = updateBookRequest.Title;
            book.Annotation = updateBookRequest.Annotation;
            book.ArtworkUrl = updateBookRequest.ArtworkUrl;

            _context.Books.Update(book);
            await _context.SaveChangesAsync();
        }
    }
}
