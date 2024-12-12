using Application.Requests;

namespace Application.Commands.Contracts;

public interface IUpdateBookCommand
{
    Task UpdateAsync(long id, UpdateBookRequest updateBookRequest, long tenantId);
}