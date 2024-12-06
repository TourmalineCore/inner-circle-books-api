using Core.Entities;

namespace Application.Queries.Contracts;

public interface IGetToDoByIdQuery
{
    Task<Book> GetByIdAsync(long id);
}