namespace Application.Requests;

public class AddBookRequest
{
    public string Title { get; set; }
    public string Annotation { get; set; }
    public long AuthorId { get; set; }
    public string Language { get; set; }
    public string ArtworkUrl { get; set; }
    public int NumberOfCopies { get; set; }
}