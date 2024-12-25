namespace Application.Commands.Contracts;

public interface IDeleteBookCommand
{
    Task DeleteAsync(long id, long tenantId);
}