using Application.Requests;

namespace Application.Commands.Contracts;

public interface ICreateAuthorCommand
{
    Task<long> CreateAsync(CreateAuthorRequest createAuthorRequest, long tenantId);
}