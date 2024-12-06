using Xunit;
using Application;
using Application.Queries;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
public class GetAllToDosQueryTests
{
    private readonly AppDbContext _context;
    private readonly GetAllToDosQuery _query;

    public GetAllToDosQueryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "GetAllToDosQueryToDoDatabase")
            .Options;

        _context = new AppDbContext(options);
        _query = new GetAllToDosQuery(_context);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllToDos()
    {
        var toDo1 = new Book { Id = 1, Name = "Test ToDo 1" };
        var toDo2 = new Book { Id = 2, Name = "Test ToDo 2" };
        var toDo3 = new Book { Id = 3, Name = "Test ToDo 3" };

        _context.ToDos.AddRange(toDo1, toDo2, toDo3);
        await _context.SaveChangesAsync();

        var result = await _query.GetAllAsync();

        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Contains(result, t => t.Id == 1 && t.Name == "Test ToDo 1");
        Assert.Contains(result, t => t.Id == 2 && t.Name == "Test ToDo 2");
        Assert.Contains(result, t => t.Id == 3 && t.Name == "Test ToDo 3");
    }
}