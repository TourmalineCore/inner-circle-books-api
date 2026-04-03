using Application;
using Application.Queries;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class GetBookByCopyIdQueryTests
{
  private const long TENANT_ID = 1;
  private readonly AppDbContext _context;
  private readonly GetBookByCopyIdQuery _query;

  public GetBookByCopyIdQueryTests()
  {
    var options = new DbContextOptionsBuilder<AppDbContext>()
      .UseInMemoryDatabase("GetBookByCopyIdQueryBooksDatabase")
      .Options;

    _context = new AppDbContext(options);
    _query = new GetBookByCopyIdQuery(_context);
  }

  [Fact]
  public async Task GetBookByCopyIdAsync_ShouldReturnBook_WhenCopyExists()
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

    _context.Books.Add(book);
    await _context.SaveChangesAsync();

    var bookCopy = new BookCopy
    {
      Id = 4,
      BookId = book.Id,
      TenantId = TENANT_ID,
      SecretKey = "abcd"
    };

    _context.BooksCopies.Add(bookCopy);
    await _context.SaveChangesAsync();

    var result = await _query.GetByCopyIdAsync(bookCopy.Id, TENANT_ID);

    Assert.Equal(book, result);
  }
}
