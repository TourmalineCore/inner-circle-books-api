using System.ComponentModel.DataAnnotations;

namespace Application.Requests;

public class UpdateAuthorRequest
{
    [MaxLength(100)]
    public string FullName { get; set; }
}