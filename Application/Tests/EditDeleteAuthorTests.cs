using Application;
using Application.Commands;
using Application.Requests;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class EditDeleteAuthorTests
{
    private readonly AppDbContext _context;

    private const long TENANT_ID = 1L;

    public EditDeleteAuthorTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "UpdateDeleteAuthorCommandBooksDatabase")
            .Options;

        _context = new AppDbContext(options);
    }

    [Fact]
    public async Task UpdateAndDeleteAsync_ShouldHaveExpectedResult()
    {
        var author1 = new Author()
        {
            Id = 1L,
            TenantId = TENANT_ID,
            Books = new List<Book>(),
            FullName = "First Demon"
        };
        var author2 = new Author()
        {
            Id = 2L,
            TenantId = TENANT_ID,
            Books = new List<Book>(),
            FullName = "Second Guy"
        };
        var author3 = new Author()
        {
            Id = 3L,
            TenantId = TENANT_ID,
            Books = new List<Book>(),
            FullName = "Third Third"
        };

    }
}