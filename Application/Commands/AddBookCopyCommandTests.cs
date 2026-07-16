using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Application.Commands;

public class AddBookCopyCommandTests
{
  private const long TENANT_ID = 1;
  private readonly AddBookCopyCommand _command;
  private readonly AppDbContext _context;

  public AddBookCopyCommandTests()
  {
    var options = new DbContextOptionsBuilder<AppDbContext>()
      .UseInMemoryDatabase("AddBookCopyCommandBooksDatabase")
      .Options;

    _context = new AppDbContext(options);
    _command = new AddBookCopyCommand(_context);
  }

  [Fact]
  public async Task ExecuteAsync_ShouldThrowWhenBookBelongsToDifferentTenant()
  {
    var otherTenantId = TENANT_ID + 1;

    var book = new Book
    {
      Id = 2,
      Title = "Other tenant book",
      Annotation = "Test annotation",
      TenantId = otherTenantId,
      CreatedAtUtc = DateTime.UtcNow,
      Language = Language.en,
      Authors = new List<Author>()
    };

    _context.Books.Add(book);
    await _context.SaveChangesAsync();

    var addBookCopyParams = new AddBookCopyCommandParams
    {
      BookId = book.Id
    };

    await Assert.ThrowsAsync<InvalidOperationException>(
      () => _command.ExecuteAsync(addBookCopyParams, TENANT_ID)
    );
  }

  [Fact]
  public async Task ExecuteAsync_ShouldThrowWhenBookNotFound()
  {
    var addBookCopyParams = new AddBookCopyCommandParams
    {
      BookId = 999
    };

    await Assert.ThrowsAsync<InvalidOperationException>(
      () => _command.ExecuteAsync(addBookCopyParams, TENANT_ID)
    );
  }
}
