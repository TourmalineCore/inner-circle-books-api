using Core.Entities;

namespace Application.Queries.Contracts;

public interface IGetAllBooksQuery
{
    Task<List<Book>> GetAllAsync();
}