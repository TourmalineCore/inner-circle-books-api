using Application.Requests;

namespace Application.Commands.Contracts;

public interface IUpdateBookCommand
{
    Task UpdateAsync(UpdateBookRequest updateBookRequest, long tenantId);
}