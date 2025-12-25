namespace Core.Entities;

public class KnowledgeArea
{
    public int Id { get; set; }
    public string Name { get; set; }

    public List<Book> Books { get; set; } = new();
}
