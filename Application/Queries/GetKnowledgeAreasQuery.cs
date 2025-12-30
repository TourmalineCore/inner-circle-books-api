using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries;

public interface IGetKnowledgeAreasQuery
{
  Task<List<KnowledgeArea>> GetByIdsAsync(List<long> ids);
  Task<List<KnowledgeArea>> GetAllKnowledgeAreasAsync();
}

public class GetKnowledgeAreasQuery: IGetKnowledgeAreasQuery
{
  private readonly AppDbContext _context;

  public GetKnowledgeAreasQuery(AppDbContext context)
  {
    _context = context;
  }
  public async Task<List<KnowledgeArea>> GetAllKnowledgeAreasAsync()
  {
    return await _context.KnowledgeAreas.ToListAsync();
  }

  public async Task<List<KnowledgeArea>> GetByIdsAsync(List<long> ids)
  {
    return await _context
      .KnowledgeAreas
      .Where(s => ids.Contains(s.Id))
      .ToListAsync();
  }
}
