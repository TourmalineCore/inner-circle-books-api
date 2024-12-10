using Application.Requests;

namespace Application.Commands.Contracts;

public interface ICreateBookCommand
{
    Task<long> CreateAsync(AddBookRequest addBookRequest);
}