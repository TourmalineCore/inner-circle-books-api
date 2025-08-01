using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Application.Queries;

public class GetCopiesIdsByBookIdQueryTests
{
    private readonly AppDbContext _context;
    private readonly GetCopiesIdsByBookIdQuery _query;

    public GetCopiesIdsByBookIdQueryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("GetCopiesIdsByBookIdQueryDatabase")
            .Options;

        _context = new AppDbContext(options);
        _query = new GetCopiesIdsByBookIdQuery(_context);
    }

    [Fact]
    public async Task GetByBookIdAsync_ShouldReturnCopiesIds()
    {
        var bookId = 1;
        var copies = new List<BookCopy>
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

        _context.BooksCopies.AddRange(copies);
        await _context.SaveChangesAsync();

        var copiesIds = await _query.GetByBookIdAsync(bookId);

        Assert.NotNull(copiesIds);
        Assert.Equal(copies.Count, copiesIds.Count);
        Assert.All(copiesIds, id => Assert.Contains(id, copies.Select(c => c.Id).ToList()));
    }
}
