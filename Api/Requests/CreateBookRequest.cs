using System.ComponentModel.DataAnnotations;

namespace Api.Requests;

public class CreateBookRequest
{
    [MaxLength(255)]
    [Required]
    public string Title { get; set; }

    [MaxLength(2000)]
    [Required]
    public string Annotation { get; set; }

    [Required]
    public List<AuthorModel> Authors { get; set; }

    [Required]
    public string Language { get; set; }

    public string BookCoverUrl { get; set; }
}