using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries
{
  public class GetKnowledgeAreasQuery
{
    private readonly AppDbContext _context;

    public GetKnowledgeAreasQuery(AppDbContext context)
    {
      _context = context;
    }

    public async Task<List<KnowledgeArea>> GetByIdsAsync(List<int> ids)
    {
      return await _context.KnowledgeAreas
        .Where(s => ids.Contains(s.Id))
        .ToListAsync();
    }
  }
}
