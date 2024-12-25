using Xunit;
using Moq;
using Application.Commands.Contracts;
using Application.Services;

public class ToDoServiceTests
{
    [Fact]
    public async Task CompleteToDoAsync_ShouldCallDeleteAsync_ForEachToDoId()
    {
        var deleteToDoCommandMock = new Mock<IDeleteToDoCommand>();
        var toDoService = new ToDoService(deleteToDoCommandMock.Object);

        var toDoIds = new List<long> { 1, 2, 3 };

        await toDoService.CompleteToDoAsync(toDoIds);

        foreach(var toDoId in toDoIds)
        {
            deleteToDoCommandMock.Verify(m => m.DeleteAsync(toDoId), Times.Once);
        }
    }
}