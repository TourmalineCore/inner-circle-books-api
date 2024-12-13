using System.ComponentModel.DataAnnotations;

namespace Application.Requests;

public class CreateAuthorRequest
{
    [MaxLength(100)]
    [Required]
    public string FullName { get; set; }
}