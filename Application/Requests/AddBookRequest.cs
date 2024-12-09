namespace Application.Requests;

public class AddBookRequest
{
    public string Title { get; set; }
    public string Annotation { get; set; }
    public string ArtworkUrl { get; set; }
    public int NumberOfCopies { get; set; }
    public long AuthorId { get; set; }
    public long LanguageId { get; set; }
}