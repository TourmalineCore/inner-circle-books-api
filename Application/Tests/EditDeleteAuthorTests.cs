using Application;
using Application.Commands;
using Application.Requests;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class EditDeleteAuthorTests
{
    private const long TENANT_ID = 1L;
    private readonly AppDbContext _context;
    private readonly DeleteAuthorCommand _deleteAuthorCommand;
    private readonly UpdateAuthorCommand _updateAuthorCommand;

    private readonly List<Author> TestDataAuthors = new()
    {
        new Author
        {
            Id = 1L,
            TenantId = TENANT_ID,
            FullName = "First Demon",
            Books = new List<Book>()
        },
        new Author
        {
            Id = 2L,
            TenantId = TENANT_ID,
            FullName = "Second Guy",
            Books = new List<Book>()
        },
        new Author
        {
            Id = 3L,
            TenantId = TENANT_ID,
            FullName = "Third Third",
            Books = new List<Book>()
        },
        new Author
        {
            Id = 4L,
            TenantId = TENANT_ID,
            FullName = "Fourth The Best",
            Books = new List<Book>()
        }
    };

    private readonly List<Book> TestDataBooks = new()
    {
        new Book
        {
            Id = 1L,
            TenantId = TENANT_ID,
            Title = "Test Book 1",
            Annotation = "Test Annotation 1",
            Language = Language.English,
            ArtworkUrl = "http://test-images.com/img404.png",
            Authors = new List<Author>()
        },
        new Book
        {
            Id = 2L,
            TenantId = TENANT_ID,
            Title = "Test Book 2",
            Annotation = "Test Annotation 2",
            Language = Language.English,
            ArtworkUrl = "http://test-images.com/img404.png",
            Authors = new List<Author>()
        }
    };

    public EditDeleteAuthorTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("UpdateDeleteAuthorCommandBooksDatabase")
            .Options;

        _context = new AppDbContext(options);
        _updateAuthorCommand = new UpdateAuthorCommand(_context);
        _deleteAuthorCommand = new DeleteAuthorCommand(_context);
        TestDataBooks[0].Authors.AddRange(TestDataAuthors);
        TestDataBooks[1].Authors.AddRange(new[] { TestDataAuthors[0], TestDataAuthors[1] });

        TestDataAuthors[0].Books.AddRange(TestDataBooks);
        TestDataAuthors[1].Books.AddRange(TestDataBooks);
        TestDataAuthors[2].Books.Add(TestDataBooks[0]);
        TestDataAuthors[3].Books.Add(TestDataBooks[0]);

        _context.Books.AddRange(TestDataBooks);
        _context.Authors.AddRange(TestDataAuthors);
        _context.SaveChangesAsync();
    }

    [Fact]
    public async Task UpdateAndDeleteAsync_ShouldHaveExpectedResult()
    {
        await _updateAuthorCommand.UpdateAsync(TestDataAuthors[0].Id,
            new UpdateAuthorRequest { FullName = "First First" }, TENANT_ID);
        await _deleteAuthorCommand.DeleteAsync(TestDataAuthors[1].Id, TENANT_ID);
        await _deleteAuthorCommand.DeleteAsync(TestDataAuthors[2].Id, TENANT_ID);

        var updatedBooks = await _context.Books.Include(b => b.Authors).ToListAsync();
        var firstAuthor = await _context.Authors.FirstAsync(a => a.Id == TestDataAuthors[0].Id);

        Assert.Equal("First First", firstAuthor.FullName);

        Assert.Equal(2, updatedBooks[0].Authors.Count);

        Assert.Single(updatedBooks[1].Authors);

        Assert.All(updatedBooks, book =>
        {
            Assert.DoesNotContain(book.Authors, a => a.Id == TestDataAuthors[1].Id);
            Assert.DoesNotContain(book.Authors, a => a.Id == TestDataAuthors[2].Id);
        });
    }
}