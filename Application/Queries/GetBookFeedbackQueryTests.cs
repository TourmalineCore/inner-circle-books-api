using Application;
using Application.Queries;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class GetBookFeedbackQueryTests
{
  private const long TENANT_ID = 1;
  private readonly AppDbContext _context;
  private readonly GetBookFeedbackQuery _query;

  public GetBookFeedbackQueryTests()
  {
    var options = new DbContextOptionsBuilder<AppDbContext>()
      .UseInMemoryDatabase("GetBookFeedbackQueryDatabase")
      .Options;

    _context = new AppDbContext(options);
    _query = new GetBookFeedbackQuery(_context);
  }

  [Fact]
  public async Task GetAnotherTenantsBookFeedback_ShouldNotGetAnotherTenantsBookFeedback()
  {
    var book = new Book
    {
      Id = 1,
      TenantId = TENANT_ID,
      Title = "Test Book",
      Annotation = "Test annotation",
      Authors = new List<Author>()
      {
        new Author()
        {
          FullName = "Test Author"
        }
      },
      Language = Language.en,
      CoverUrl = "http://test-images.com/img404.png"
    };

    var bookFeedback = new BookFeedback
    {
      Id = 1,
      BookId = book.Id,
      Book = book,
      TenantId = TENANT_ID
    };

    await _context.BookFeedback.AddAsync(bookFeedback);
    await _context.SaveChangesAsync();

    var result = await _query.GetAsync(bookFeedback.BookId, 777);

    Assert.DoesNotContain(result, x => x.Id == bookFeedback.Id);
  }

  [Fact]
  public async Task GetFeedbackForDeletedBook_ShouldNotGetFeedbackForDeletedBook()
  {
    var book = new Book
    {
      Id = 2,
      TenantId = TENANT_ID,
      Title = "Test Book",
      Annotation = "Test annotation",
      Authors = new List<Author>()
      {
        new Author()
        {
          FullName = "Test Author"
        }
      },
      Language = Language.en,
      CoverUrl = "http://test-images.com/img404.png",
      DeletedAtUtc = DateTime.UtcNow
    };

    var bookFeedback = new BookFeedback
    {
      Id = 2,
      BookId = book.Id,
      Book = book,
      TenantId = TENANT_ID
    };

    await _context.BookFeedback.AddAsync(bookFeedback);
    await _context.SaveChangesAsync();

    var result = await _query.GetAsync(bookFeedback.BookId, TENANT_ID);

    Assert.DoesNotContain(result, x => x.Id == bookFeedback.Id);
  }

  [Fact]
  public async Task GetBookFeedbackForNonExistingBook_ShouldNotGetFeedbackForNonExistingBook()
  {
    var nonExistentBookId = 999;

    var result = await _query.GetAsync(nonExistentBookId, TENANT_ID);

    Assert.DoesNotContain(result, x => x.Id == nonExistentBookId);
  }
}
