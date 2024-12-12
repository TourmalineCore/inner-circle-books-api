using Application.Requests;

namespace Application.Commands.Contracts;

public interface IUpdateAuthorCommand
{
    Task UpdateAsync(long id, UpdateAuthorRequest updateBookRequest, long tenantId);
}