using System.ComponentModel.DataAnnotations;

namespace Application.Requests;

public class UpdateBookRequest
{
    [Required]
    [MaxLength(255)]
    public string Title { get; set; }

    [MaxLength(2000)]
    public string Annotation { get; set; }
    [Required]
    public string Language { get; set; }
    [Required]
    public List<AuthorModel> Authors { get; set; }

    public string BookCoverUrl { get; set; }
}