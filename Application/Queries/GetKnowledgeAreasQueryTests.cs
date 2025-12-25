using Application;
using Application.Queries;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class GetKnowledgeAreasQueryTests
{
  private readonly AppDbContext _context;
  private readonly GetKnowledgeAreasQuery _query;
  public GetKnowledgeAreasQueryTests()
  {
    var options = new DbContextOptionsBuilder<AppDbContext>()
      .UseInMemoryDatabase($"GetKnowledgeAreasDatabase_{Guid.NewGuid()}")
      .Options;

    _context = new AppDbContext(options);
    _query = new GetKnowledgeAreasQuery(_context);

    _context.KnowledgeAreas.AddRange(
      new KnowledgeArea { Id = 1, Name = "Frontend" },
      new KnowledgeArea { Id = 2, Name = "Backend" },
      new KnowledgeArea { Id = 3, Name = "QA" }
    );
    _context.SaveChanges();
  }

  [Fact]
  public async Task GetByIdsAsync_ShouldReturnOnlyRequestedKnowledgeAreas()
  {
    var result = await _query.GetByIdsAsync(new List<int> { 1, 3 });

    Assert.Equal(2, result.Count);
    Assert.Contains(result, s => s.Id == 1 && s.Name == "Frontend");
    Assert.Contains(result, s => s.Id == 3 && s.Name == "QA");
  }

  [Fact]
  public async Task GetByIdsAsync_ShouldReturnEmpty_WhenNoIdsMatch()
  {
    var result = await _query.GetByIdsAsync(new List<int> { 4, 5 });

    Assert.Empty(result);
  }

  [Fact]
  public async Task GetByIdsAsync_ShouldReturnEmpty_WhenInputListIsEmpty()
  {
    var result = await _query.GetByIdsAsync(new List<int>());

    Assert.Empty(result);
  }
}
