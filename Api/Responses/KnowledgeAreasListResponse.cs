public class KnowledgeAreasListResponse
{
  public List<KnowledgeAreaItem> KnowledgeAreas { get; set; }
}

public class KnowledgeAreaItem
{
  public long Id { get; set; }
  
  public string Name { get; set; }
}
