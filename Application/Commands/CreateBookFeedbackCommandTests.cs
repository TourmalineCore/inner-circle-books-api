using Core;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Application.Commands;

public class CreateBookFeedbackCommandTests
{
  private const long TENANT_ID = 1;
  private readonly CreateBookFeedbackCommand _command;
  private readonly AppDbContext _context;

  public CreateBookFeedbackCommandTests()
  {
    var options = new DbContextOptionsBuilder<AppDbContext>()
      .UseInMemoryDatabase("CreateBookFeedbackCommandDatabase")
      .Options;

    _context = new AppDbContext(options);
    _command = new CreateBookFeedbackCommand(_context);
  }

  [Fact]
  public async Task ExecuteAsync_ShouldThrowWhenBookForFeedbackBelongsToDifferentTenant()
  {
    var otherTenantId = TENANT_ID + 1;

    var employee = new Employee
    {
      Id = 2
    };
    
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

    var createBookFeedbackCommandParams = new CreateBookFeedbackCommandParams
    {
      BookId = book.Id
    };

    await Assert.ThrowsAsync<InvalidOperationException>(
      () => _command.ExecuteAsync(createBookFeedbackCommandParams, employee, TENANT_ID)
    );
  }

  [Fact]
  public async Task ExecuteAsync_ShouldThrowWhenBookForFeedbackNotFound()
  {
    var employee = new Employee
    {
      Id = 2
    };

    var createBookFeedbackCommandParams = new CreateBookFeedbackCommandParams
    {
      BookId = 999
    };

    await Assert.ThrowsAsync<InvalidOperationException>(
      () => _command.ExecuteAsync(createBookFeedbackCommandParams, employee, TENANT_ID)
    );
  }
}
