using Core.Entities;

namespace Application.Queries.Contracts;

public interface IGetAllToDosQuery
{
    Task<List<ToDo>> GetAllAsync();
}