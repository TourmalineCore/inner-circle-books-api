using System.ComponentModel.DataAnnotations;

namespace Application.Requests;

public class UpdateBookRequest
{
    [MaxLength(255)]
    public string Title { get; set; }

    [MaxLength(2000)]
    public string Annotation { get; set; }

    public string Language { get; set; }

    public List<string> Authors { get; set; }

    public string ArtworkUrl { get; set; }
}