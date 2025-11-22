namespace Core.Entities;

public class Book
{
  public long Id { get; set; }

  public long TenantId { get; set; }

  public string Title { get; set; }

  public string Annotation { get; set; }

  public List<Author> Authors { get; set; }

  public Language Language { get; set; }

  public DateTime CreatedAtUtc { get; set; }

  public DateTime? DeletedAtUtc { get; set; } = null;

  public string? CoverUrl { get; set; }

  public List<BookCopy> Copies { get; set; }
}
