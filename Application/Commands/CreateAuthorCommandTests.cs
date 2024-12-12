using Application.Requests;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Application.Commands
{
    public class CreateAuthorCommandTests
    {
        private readonly AppDbContext _context;
        private readonly CreateAuthorCommand _command;

        private const long TENANT_ID = 1L;

        public CreateAuthorCommandTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "CreateAuthorCommandBooksDatabase")
                .Options;

            _context = new AppDbContext(options);
            _command = new CreateAuthorCommand(_context);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddNewAuthorToDbSet()
        {
            var addAuthorRequest = new CreateAuthorRequest
            {
                FullName = "Test Author"
            };

            var authorId = await _command.CreateAsync(addAuthorRequest, TENANT_ID);

            var author = await _context.Authors.FindAsync(authorId);
            Assert.NotNull(author);
            Assert.Equal("Test Author", author.FullName);
            Assert.Equal(authorId, author.Id);
        }
    }
}