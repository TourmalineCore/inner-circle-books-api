using Api.Responses;
using Application.Commands.Contracts;
using Application.Queries.Contracts;
using Application.Requests;
using Application.Services;
using Moq;
using Xunit;

namespace Api.Controllers
{
    public class ToDoControllerTests
    {
        private readonly Mock<IGetAllToDosQuery> _getAllToDosQueryMock;
        private readonly Mock<ICreateToDoCommand> _createToDoCommandMock;
        private readonly Mock<IDeleteToDoCommand> _deleteToDoCommandMock;
        private readonly Mock<ISoftDeleteToDoCommand> _softDeleteToDoCommandMock;
        private readonly Mock<ToDoService> _toDoServiceMock;
        private readonly ToDoController _controller;

        public ToDoControllerTests()
        {
            _getAllToDosQueryMock = new Mock<IGetAllToDosQuery>();
            _createToDoCommandMock = new Mock<ICreateToDoCommand>();
            _deleteToDoCommandMock = new Mock<IDeleteToDoCommand>();
            _softDeleteToDoCommandMock = new Mock<ISoftDeleteToDoCommand>();
            _toDoServiceMock = new Mock<ToDoService>(new Mock<IDeleteToDoCommand>().Object);

            _controller = new ToDoController(
                _toDoServiceMock.Object,
                _getAllToDosQueryMock.Object,
                _createToDoCommandMock.Object,
                _deleteToDoCommandMock.Object,
                _softDeleteToDoCommandMock.Object
            );
        }

        [Fact]
        public async Task GetAllToDosAsync_ShouldReturnToDosResponse()
        {
            var toDoItems = new List<Core.Entities.ToDo>
            {
                new () { Id = 1, Name = "Test ToDo 1" },
                new()  { Id = 2, Name = "Test ToDo 2" }
            };
            _getAllToDosQueryMock
                .Setup(query => query.GetAllAsync())
                .ReturnsAsync(toDoItems);

            var result = await _controller.GetAllToDosAsync();

            Assert.IsType<ToDosResponse>(result);
            Assert.Equal(2, result.ToDos.Count);
            Assert.Equal("Test ToDo 1", result.ToDos[0].Name);
            _getAllToDosQueryMock.Verify(query => query.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task AddToDoAsync_ShouldReturnCreatedToDoId()
        {
            var request = new AddToDoRequest { Name = "New ToDo" };
            _createToDoCommandMock
                .Setup(command => command.CreateAsync(request))
                .ReturnsAsync(1);

            var result = await _controller.AddToDoAsync(request);

            Assert.Equal(1, result);
            _createToDoCommandMock.Verify(command => command.CreateAsync(request), Times.Once);
        }

        [Fact]
        public async Task CompleteToDo_ShouldCallCompleteToDoAsync()
        {
            var request = new CompleteToDoRequest { ToDoIds = new List<long> { 1, 2 } };

            await _controller.CompleteToDo(request);

            _toDoServiceMock.Verify(service => service.CompleteToDoAsync(request.ToDoIds), Times.Once);
        }

        [Fact]
        public async Task DeleteToDo_ShouldCallDeleteAsync()
        {
            var toDoId = 1L;

            await _controller.DeleteToDo(toDoId);

            _deleteToDoCommandMock.Verify(command => command.DeleteAsync(toDoId), Times.Once);
        }

    }
}
