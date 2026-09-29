using Application;
using Application.Queries;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class GetBookByIdQueryTests
{
  private const long TENANT_ID = 1;
  private readonly AppDbContext _context;
  private readonly GetBookByIdQuery _query;

  public GetBookByIdQueryTests()
  {
    var options = new DbContextOptionsBuilder<AppDbContext>()
      .UseInMemoryDatabase("GetBookByIdQueryBooksDatabase")
      .Options;

    _context = new AppDbContext(options);
    _query = new GetBookByIdQuery(_context);
  }

  [Fact]
  public async Task GetByIdAsync_ShouldReturnNull_WhenBookDoesNotExist()
  {
    var nonExistentId = 999;

    var result = await _query.GetByIdAsync(nonExistentId, TENANT_ID);

    Assert.Null(result);
  }

  [Fact]
  public async Task GetByIdAsync_ShouldReturnNull_WhenBookBelongsToAnotherTenant()
  {
    var book = new Book
    {
      TenantId = TENANT_ID,
      Title = "Test Book 1",
      Annotation = "Test annotation 1",
      Authors = new List<Author>()
      {
        new Author()
        {
          FullName = "Test Author"
        }
      },
      Language = Language.en,
      CoverUrl = ""
    };

    _context.Books.Add(book);
  
    await _context.SaveChangesAsync();

    var result = await _query.GetByIdAsync(book.Id, 999);

    Assert.Null(result);
  }

  [Fact]
  public async Task GetBookIdAsync_ShouldReturnNull_WhenBookIsDeleted()
  {
    var book = new Book
    {
      TenantId = TENANT_ID,
      Title = "Test Book 1",
      Annotation = "Test annotation 1",
      Authors = new List<Author>()
      {
        new Author()
        {
          FullName = "Test Author"
        }
      },
      DeletedAtUtc = DateTime.UtcNow,
      Language = Language.en,
      CoverUrl = ""
    };

    _context.Books.Add(book);
    await _context.SaveChangesAsync();

    var result = await _query.GetByIdAsync(book.Id, TENANT_ID);

    Assert.Null(result);
  }
}
