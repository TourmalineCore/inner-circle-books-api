using Microsoft.EntityFrameworkCore;

namespace Application;

public static class QueryableExtensions
{
  public static IQueryable<TEntity> QueryableAsNoTracking<TEntity>(this DbContext context) where TEntity : class
  {
    return context.Queryable<TEntity>().AsNoTracking();
  }

  public static IQueryable<TEntity> Queryable<TEntity>(this DbContext context) where TEntity : class
  {
    return context.Set<TEntity>();
  }
}
