using Core.Entities;

namespace Application.Commands;

public class AddBookCopyCommandParams
{
    public long BookId { get; set; }
}

public class AddBookCopyCommand
{
  private readonly AppDbContext _context;

  public AddBookCopyCommand(AppDbContext context)
  {
    _context = context;
  }

  public async Task ExecuteAsync(AddBookCopyCommandParams addBookCopyCommandParams, long tenantId)
  {
    var newBookCopy = new BookCopy()
    {
        TenantId = tenantId,
        SecretKey = SecretKeyGenerator.Generate(),
        BookId = addBookCopyCommandParams.BookId,
    };
    
    await _context
        .Set<BookCopy>()
        .AddAsync(newBookCopy);

    await _context.SaveChangesAsync();
  }
}
