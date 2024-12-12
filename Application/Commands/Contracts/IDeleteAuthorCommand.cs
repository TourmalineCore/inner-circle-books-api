namespace Application.Commands.Contracts;

public interface IDeleteAuthorCommand
{
    Task DeleteAsync(long id, long tenantId);
}