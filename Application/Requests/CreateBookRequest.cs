using System.ComponentModel.DataAnnotations;
using Core.Entities;

namespace Application.Requests;

public class CreateBookRequest
{
    [MaxLength(255)]
    [Required]
    public string Title { get; set; }

    [MaxLength(2000)]
    public string Annotation { get; set; }

    [Required]
    public List<AuthorModel> Authors { get; set; }

    [Required]
    public string Language { get; set; }

    public string BookCoverUrl { get; set; }
}