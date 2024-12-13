using Application.Commands.Contracts;
using Application.Requests;
using Core.Entities;

namespace Application.Commands;

public class CreateBookCommand : ICreateBookCommand
{
    private readonly ICreateAuthorCommand _command;
    private readonly AppDbContext _context;

    public CreateBookCommand(
        AppDbContext context,
        ICreateAuthorCommand command)
    {
        _context = context;
        _command = command;
    }

    public async Task<long> CreateAsync(CreateBookRequest createBookRequest, long tenantId)
    {
        if (createBookRequest.Authors == null || createBookRequest.Authors.Count == 0)
        {
            throw new ArgumentException("List of authors cannot be empty or null.");
        }

        var authors = new List<Author>();
        foreach (var authorFullName in createBookRequest.Authors)
        {
            if (!_context.Authors.Any(x => x.FullName == authorFullName && x.TenantId == tenantId))
            {
                var createAuthorRequest = new CreateAuthorRequest
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
            createBookRequest.Title,
            createBookRequest.Annotation,
            (Language)Enum.Parse(typeof(Language), createBookRequest.Language),
            authors,
            createBookRequest.ArtworkUrl);
        await _context.Books.AddAsync(book);
        await _context.SaveChangesAsync();
        return book.Id;
    }
}