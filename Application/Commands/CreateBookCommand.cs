using Core.Entities;

namespace Application.Commands;

public class CreateBookCommandParams
{
  public string Title { get; set; }

  public string Annotation { get; set; }

  public List<Author> Authors { get; set; }

  public Language Language { get; set; }

  public List<Specialization> Specializations { get; set; }

  public string CoverUrl { get; set; }

  public int CountOfCopies { get; set; }
}

public class CreateBookCommand
{
  private readonly AppDbContext _context;

  private static readonly Random _random = new Random();

  private const string CHARS = "abcdefghijklmnopqrstuvwxyz0123456789";

  public CreateBookCommand(AppDbContext context)
  {
    _context = context;
  }

  private string GenerateSecretKey()
  {
    return new string(Enumerable
      .Repeat(CHARS, 4)
      .Select(s => s[_random.Next(s.Length)]).ToArray());
  }

  public async Task<long> CreateAsync(CreateBookCommandParams createBookCommandParams, long tenantId)
  {
    if (createBookCommandParams.Authors == null || createBookCommandParams.Authors.Count == 0)
    {
      throw new ArgumentException("List of authors cannot be empty or null.");
    }

    var book = new Book
    {
      TenantId = tenantId,
      Title = createBookCommandParams.Title,
      Annotation = createBookCommandParams.Annotation,
      Specializations = createBookCommandParams.Specializations,
      Authors = createBookCommandParams
        .Authors
        .Select(x => new Author()
        {
          FullName = x.FullName
        })
        .ToList(),
      Language = createBookCommandParams.Language,
      CoverUrl = createBookCommandParams.CoverUrl,
      CreatedAtUtc = DateTime.UtcNow,
      Copies = Enumerable
        .Range(0, createBookCommandParams.CountOfCopies)
        .Select(x => new BookCopy()
        {
          TenantId = tenantId,
          SecretKey = GenerateSecretKey()
        })
        .ToList()
    };

    await _context.Books.AddAsync(book);
    await _context.SaveChangesAsync();

    return book.Id;
  }
}
