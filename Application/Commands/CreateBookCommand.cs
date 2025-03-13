using Core.Entities;

namespace Application.Commands;

public class CreateBookCommandParams
{
    public string Title { get; set; }

    public string Annotation { get; set; }

    public List<Author> Authors { get; set; }

    public string Language { get; set; }

    public string BookCoverUrl { get; set; }
}

public class CreateBookCommand
{
    private readonly AppDbContext _context;

    public CreateBookCommand(AppDbContext context)
    {
        _context = context;
    }

    public async Task<long> CreateAsync(CreateBookCommandParams createBookCommandParams, long tenantId)
    {
        if (createBookCommandParams.Authors == null || createBookCommandParams.Authors.Count == 0)
        {
            throw new ArgumentException("List of authors cannot be empty or null.");
        }

        var book = new Book(
            tenantId,
            createBookCommandParams.Title,
            createBookCommandParams.Annotation,
            createBookCommandParams.Authors.Select(x => new Author() { FullName = x.FullName }).ToList(),
            (Language)Enum.Parse(typeof(Language), createBookCommandParams.Language),
            createBookCommandParams.BookCoverUrl);

        await _context.Books.AddAsync(book);
        await _context.SaveChangesAsync();

        return book.Id;
    }
}