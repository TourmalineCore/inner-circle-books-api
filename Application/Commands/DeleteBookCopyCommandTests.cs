using Application;
using Application.Commands;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class DeleteBookCopyCommandTests
{
    private const long TENANT_ID = 1;
    private readonly DeleteBookCopyCommand _command;
    private readonly AppDbContext _context;

    public DeleteBookCopyCommandTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("DeleteBookCopyCommandBooksDatabase")
            .Options;

        _context = new AppDbContext(options);
        _command = new DeleteBookCopyCommand(_context);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteBookCopyFromDbSet()
    {
        var bookCopy = new BookCopy
        {
            Id = 1,
            TenantId = TENANT_ID,
            BookId = 1,
            SecretKey = "abcd"
        };

        _context.BooksCopies.Add(bookCopy);
        await _context.SaveChangesAsync();

        await _command.DeleteAsync(bookCopy.Id, bookCopy.TenantId);

        var isBookCopyExistInDbSet = await _context.BooksCopies
            .Where(x => x.TenantId == TENANT_ID)
            .AnyAsync(x => x.Id == bookCopy.Id);

        Assert.False(isBookCopyExistInDbSet);
    }
}