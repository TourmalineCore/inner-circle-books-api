using Application;
using Application.Queries;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class BookCopyValidatorQueryTests
{
  private const long TENANT_ID = 1;
  private readonly AppDbContext _context;
  private readonly BookCopyValidatorQuery _query;

  public BookCopyValidatorQueryTests()
  {
    var options = new DbContextOptionsBuilder<AppDbContext>()
      .UseInMemoryDatabase("BookCopyValidatorDatabase")
      .Options;

    _context = new AppDbContext(options);
    _query = new BookCopyValidatorQuery(_context);
  }

  [Fact]
  public async Task IsValidSecretKeyAsync_ShouldReturnTrue_IfSecretKeyIsValid()
  {
    var bookCopy = new BookCopy
    {
      Id = 1,
      TenantId = TENANT_ID,
      BookId = 1,
      SecretKey = "a3fr"
    };

    _context.BooksCopies.Add(bookCopy);

    await _context.SaveChangesAsync();

    var result = await _query.IsValidSecretKeyAsync(bookCopy.Id, bookCopy.SecretKey, TENANT_ID);

    Assert.True(result);
  }

  [Fact]
  public async Task IsValidSecretKeyAsync_ShouldReturnFalseIfSecretKeyIsNotValid()
  {
    var notValidSecretKey = "eewf";

    var bookCopy = new BookCopy
    {
      Id = 2,
      TenantId = TENANT_ID,
      BookId = 1,
      SecretKey = "a3fr"
    };

    _context.BooksCopies.Add(bookCopy);

    await _context.SaveChangesAsync();

    var result = await _query.IsValidSecretKeyAsync(bookCopy.Id, notValidSecretKey, TENANT_ID);

    Assert.False(result);
  }
}
