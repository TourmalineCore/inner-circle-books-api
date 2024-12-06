using Core.Entities;

namespace Application.Queries.Contracts;

public interface IGetAllToDosQuery
{
    Task<List<Book>> GetAllAsync();
}