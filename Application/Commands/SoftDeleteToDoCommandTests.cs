using Xunit;
using Moq;
using Application;
using Application.Commands;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using NodaTime;

public class SoftDeleteToDoCommandTests
{
    private readonly AppDbContext _context;
    private readonly Mock<IClock> _clockMock;
    private readonly SoftDeleteToDoCommand _command;

    public SoftDeleteToDoCommandTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "SoftDeleteToDoCommandToDoDatabase")
            .Options;

        _context = new AppDbContext(options);

        _clockMock = new Mock<IClock>();
        _command = new SoftDeleteToDoCommand(_context, _clockMock.Object);
    }

    [Fact]
    public async Task SoftDeleteAsync_ShouldSetDeletedAtUtc()
    {
        var currentInstant = Instant.FromUtc(2024, 8, 28, 12, 0, 0);
        _clockMock.Setup(c => c.GetCurrentInstant()).Returns(currentInstant);

        var toDo = new Book { Id = 1, Name = "Test ToDo", DeletedAtUtc = null };
        _context.ToDos.Add(toDo);
        await _context.SaveChangesAsync();

        await _command.SoftDeleteAsync(toDo.Id);

        var updatedToDo = await _context.ToDos.FindAsync(toDo.Id);
        Assert.NotNull(updatedToDo);
        Assert.Equal(currentInstant.ToDateTimeUtc(), updatedToDo.DeletedAtUtc);
    }
}