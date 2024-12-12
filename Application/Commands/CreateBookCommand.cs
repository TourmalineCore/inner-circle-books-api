using Application.Commands.Contracts;
using Application.Requests;
using Core.Entities;

namespace Application.Commands
{
    public class CreateBookCommand : ICreateBookCommand
    {
        private readonly AppDbContext _context;
        private readonly ICreateAuthorCommand _command;

        public CreateBookCommand(
            AppDbContext context, 
            ICreateAuthorCommand command)
        {
            _context = context;
            _command = command;
        }

        public async Task<long> CreateAsync(AddBookRequest addBookRequest, long tenantId)
        {
            var authors = new List<Author>();
            foreach (var authorFullName in addBookRequest.Authors)
            {
                if (!_context.Authors.Any(x => x.FullName == authorFullName && x.TenantId == tenantId))
                {
                    var createAuthorRequest = new CreateAuthorRequest()
                    {
                        FullName = authorFullName
                    };
                    await _command.CreateAsync(createAuthorRequest, tenantId);
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