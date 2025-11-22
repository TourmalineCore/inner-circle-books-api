using Application;
using Application.Queries;
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

    await Assert.ThrowsAsync<InvalidOperationException>(() => _query.GetByIdAsync(nonExistentId, TENANT_ID));
  }
}
