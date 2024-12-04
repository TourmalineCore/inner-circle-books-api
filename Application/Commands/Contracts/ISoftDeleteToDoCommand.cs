namespace Application.Commands.Contracts;

public interface ISoftDeleteToDoCommand
{
    Task SoftDeleteAsync(long id);
}