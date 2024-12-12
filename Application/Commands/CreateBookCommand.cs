using Application.Commands.Contracts;
using Application.Requests;
using Core.Entities;

namespace Application.Commands
{
    public class CreateBookCommand : ICreateBookCommand
    {
        private readonly AppDbContext _context;

        public CreateBookCommand(AppDbContext context)
        {
            _context = context;
        }

        public async Task<long> CreateAsync(AddBookRequest addBookRequest, long tenantId)
        {
            var authors = new List<Author>();
            foreach (var authorFullName in addBookRequest.Authors)
            {
                if (!_context.Authors.Any(x => x.FullName == authorFullName && x.TenantId == tenantId))
                {
                    _context.Authors.Add(new Author(tenantId, authorFullName));
                    await _context.SaveChangesAsync();
                }

                var existAuthor = _context.Authors.Single(x => x.FullName == authorFullName && x.TenantId == tenantId);
                authors.Add(existAuthor);
                await _context.SaveChangesAsync();
            }

            var book = new Book(
                tenantId,
                addBookRequest.Title,
                addBookRequest.Annotation,
                (Language)Enum.Parse(typeof(Language), addBookRequest.Language),
                authors,
                addBookRequest.ArtworkUrl);
            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();
            return book.Id;
        }
    }
}