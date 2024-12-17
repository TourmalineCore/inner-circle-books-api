using System.ComponentModel.DataAnnotations;

namespace Application.Requests;

public class CreateBookRequest
{
    [MaxLength(255)]
    [Required]
    public string Title { get; set; }

    [MaxLength(2000)]
    public string Annotation { get; set; }

    [Required]
    public List<string> Authors { get; set; }

    [Required]
    public string Language { get; set; }

    public string ArtworkUrl { get; set; }
}