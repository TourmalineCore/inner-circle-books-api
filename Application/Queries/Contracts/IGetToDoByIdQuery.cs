using Core.Entities;

namespace Application.Queries.Contracts;

public interface IGetToDoByIdQuery
{
    Task<ToDo> GetByIdAsync(long id);
}