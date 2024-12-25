using Core.Entities;

namespace Application.Queries.Contracts;

public interface IGetBookByIdQuery
{
    Task<Book> GetByIdAsync(long id, long tenantId);
}