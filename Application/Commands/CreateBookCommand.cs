using Application.Commands.Contracts;
using Application.Requests;
using Core.Entities;

namespace Application.Commands;

public class CreateBookCommand : ICreateBookCommand
{
    private readonly AppDbContext _context;

    public CreateBookCommand(AppDbContext context)
    {
        _context = context;
    }

    public async Task<long> CreateAsync(CreateBookRequest createBookRequest, long tenantId)
    {
        if (createBookRequest.Authors == null || createBookRequest.Authors.Count == 0)
        {
            throw new ArgumentException("List of authors cannot be empty or null.");
        }

        var book = new Book(
            tenantId,
            createBookRequest.Title,
            createBookRequest.Annotation,
            createBookRequest.Authors.Select(x => new Author() { FullName = x.FullName }).ToList(),
            (Language)Enum.Parse(typeof(Language), createBookRequest.Language),
            createBookRequest.BookCoverUrl);

        await _context.Books.AddAsync(book);
        await _context.SaveChangesAsync();

        return book.Id;
    }
}