namespace Application.Commands.Contracts;

public interface IDeleteToDoCommand
{
    Task DeleteAsync(long id);
}