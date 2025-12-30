public class KnowledgeAreasListResponse
{
  public List<KnowledgeAreaResponse> KnowledgeAreas { get; set; }
}

public class KnowledgeAreaResponse
{
  public long Id { get; set; }
  public string Name { get; set; }
}
