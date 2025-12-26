namespace Core.Entities;

public class KnowledgeArea
{
    public long Id { get; set; }
    public string Name { get; set; }

    public List<Book> Books { get; set; } = new();
}
