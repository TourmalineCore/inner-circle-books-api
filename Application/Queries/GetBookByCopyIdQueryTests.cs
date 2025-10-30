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
    public async Task GetBookIdByCopyIdAsync_ShouldReturnBookId_WhenCopyExists()
    {
        var bookId = 1;

        var bookCopy = new BookCopy
        {
            Id = 4,
            BookId = bookId,
            TenantId = TENANT_ID,
            SecretKey = "abcd"
        };

        _context.BooksCopies.Add(bookCopy);
        await _context.SaveChangesAsync();

        var result = await _query.GetBookIdByCopyIdAsync(bookCopy.Id, TENANT_ID);

        Assert.Equal(bookId, result);
    }
}