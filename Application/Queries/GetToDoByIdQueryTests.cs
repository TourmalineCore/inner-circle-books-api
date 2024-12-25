using Xunit;
using Application;
using Application.Queries;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

public class GetToDoByIdQueryTests
{
    private readonly AppDbContext _context;
    private readonly GetToDoByIdQuery _query;

    public GetToDoByIdQueryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "GetToDoByIdQueryToDoDatabase")
            .Options;

        _context = new AppDbContext(options);
        _query = new GetToDoByIdQuery(_context);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnToDo_WhenToDoExists()
    {
        var toDo = new ToDo { Id = 1, Name = "Test ToDo" };
        _context.ToDos.Add(toDo);
        await _context.SaveChangesAsync();

        var result = await _query.GetByIdAsync(toDo.Id);

        Assert.NotNull(result);
        Assert.Equal(toDo.Id, result.Id);
        Assert.Equal(toDo.Name, result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenToDoDoesNotExist()
    {
        var nonExistentId = 999;

        var result = await _query.GetByIdAsync(nonExistentId);

        Assert.Null(result);
    }
}