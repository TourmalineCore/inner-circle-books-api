using Application.Commands.Contracts;
using Application.Requests;
using Core.Entities;
using NodaTime;

namespace Application.Commands
{
    public class CreateBookCommand : ICreateBookCommand
    {
        private readonly AppDbContext _context;

        public CreateBookCommand(AppDbContext context)
        {
            _context = context;
        }

        public async Task<long> CreateAsync(AddBookRequest addBookRequest)
        {
            var book = new Book(
                addBookRequest.Title, 
                addBookRequest.Annotation,
                (Language)Enum.Parse(typeof(Language), addBookRequest.Language),
                addBookRequest.AuthorId,
                addBookRequest.ArtworkUrl, 
                addBookRequest.NumberOfCopies);
            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();
            return book.Id;
        }
    }
}