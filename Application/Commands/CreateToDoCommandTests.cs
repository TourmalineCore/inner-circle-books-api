using Xunit;
using Moq;
using Application.Requests;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace Application.Commands.Tests
{
    public class CreateToDoCommandTests
    {
        private readonly Mock<IClock> _clockMock;
        private readonly AppDbContext _context;
        private readonly CreateToDoCommand _command;

        public CreateToDoCommandTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "CreateToDoCommandToDoDatabase")
                .Options;

            _context = new AppDbContext(options);
            _clockMock = new Mock<IClock>();

            _command = new CreateToDoCommand(_context, _clockMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddNewToDoToDbSet()
        {
            var addToDoRequest = new AddToDoRequest { Name = "Test ToDo" };

            var toDoId = await _command.CreateAsync(addToDoRequest);

            var toDo = await _context.ToDos.FindAsync(toDoId);
            Assert.NotNull(toDo);
            Assert.Equal("Test ToDo", toDo.Name);
            Assert.Equal(toDoId, toDo.Id);
        }
    }
}
