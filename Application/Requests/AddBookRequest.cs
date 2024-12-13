namespace Application.Requests;

public class AddBookRequest
{
    public string Title { get; set; }

    public string Annotation { get; set; }

    public List<string> Authors { get; set; }

    public string Language { get; set; }

    public string ArtworkUrl { get; set; }
}