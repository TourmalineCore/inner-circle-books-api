using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Application.Queries;

public class GetBookCopiesByBookIdQueryTests
{
    private readonly AppDbContext _context;
    private readonly GetBookCopiesByBookIdQuery _query;

    public GetBookCopiesByBookIdQueryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("GetBookCopiesByBookIdQueryDatabase")
            .Options;

        _context = new AppDbContext(options);
        _query = new GetBookCopiesByBookIdQuery(_context);
    }

    [Fact]
    public async Task GetByBookIdAsync_ShouldReturnBookCopies()
    {
        var bookId = 1;
        var bookCopies = new List<BookCopy>
        {
            new BookCopy {
                Id = 1,
                BookId = bookId,
            },
            new BookCopy {
                Id = 2,
                BookId = bookId,
            }
        };

        _context.BooksCopies.AddRange(bookCopies);
        await _context.SaveChangesAsync();

        var result = await _query.GetByBookIdAsync(bookId);

        Assert.NotNull(result);
        Assert.Equal(bookCopies.Count, result.Count);
        Assert.All(result, copy => Assert.Equal(bookId, copy.BookId));
    }
}
