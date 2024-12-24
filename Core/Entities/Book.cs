namespace Core.Entities;

public class Book
{
    public Book(
        long tenantId,
        string title,
        string annotation,
        List<Author> authors,
        Language language,
        string artworkUrl)
    {
        TenantId = tenantId;
        Title = title;
        Annotation = annotation;
        Authors = authors;
        Language = language;
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

    public List<Author> Authors { get; set; }

    public Language Language { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? DeletedAtUtc { get; set; } = null;

    public string ArtworkUrl { get; set; }
}