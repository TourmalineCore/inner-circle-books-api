using Application.Requests;

namespace Application.Commands.Contracts;

public interface IEditBookCommand
{
    Task EditAsync(long id, EditBookRequest editBookRequest, long tenantId);
}