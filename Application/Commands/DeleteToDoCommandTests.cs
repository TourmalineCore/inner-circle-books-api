using Xunit;
using Application;
using Application.Commands;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

public class DeleteToDoCommandTests
{
    private readonly AppDbContext _context;
    private readonly DeleteToDoCommand _command;

    public DeleteToDoCommandTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "DeleteToDoCommandToDoDatabase")
            .Options;

        _context = new AppDbContext(options);
        _command = new DeleteToDoCommand(_context);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveToDoFromDbSet()
    {
        var toDo = new ToDo { Id = 1, Name = "Test ToDo" };
        _context.ToDos.Add(toDo);
        await _context.SaveChangesAsync();

        await _command.DeleteAsync(toDo.Id);

        var deletedToDo = await _context.ToDos.FindAsync(toDo.Id);
        Assert.Null(deletedToDo);
    }
}