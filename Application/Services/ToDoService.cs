using Application.Commands.Contracts;

namespace Application.Services
{
    public class ToDoService
    {
        private readonly IDeleteToDoCommand _deleteToDoCommand;
        public ToDoService(IDeleteToDoCommand deleteToDoCommand)
        {
            _deleteToDoCommand = deleteToDoCommand;
        }

        // For tests
        public ToDoService() { }
        public virtual async Task CompleteToDoAsync(List<long> toDoIds)
        {
            foreach (var toDoId in toDoIds)
            {
                await _deleteToDoCommand.DeleteAsync(toDoId);
            }
        }
    }
}