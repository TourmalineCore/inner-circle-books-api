using Application.Requests;

namespace Application.Commands.Contracts;

public interface IUpdateAuthorCommand
{
    Task UpdateAsync(UpdateAuthorRequest updateBookRequest, long tenantId);
}