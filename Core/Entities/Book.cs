namespace Core.Entities;

public class Book
{
    public Book(
        long tenantId,
        string title,
        string annotation,
        Language language,
        List<Author> authors,
        string artworkUrl)
    {
        TenantId = tenantId;
        Title = title;
        Annotation = annotation;
        Language = language;
        Authors = authors;
        ArtworkUrl = artworkUrl;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Book()
    {
    }

    public long Id { get; set; }

    public long TenantId { get; set; }

    public string Title { get; set; }

    public string Annotation { get; set; }

    public Language Language { get; set; }

    public List<Author> Authors { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? DeletedAtUtc { get; set; } = null;

    public string ArtworkUrl { get; set; }

    public bool DeleteAuthor(Author author)
    {
        return Authors.Remove(author);
    }
}