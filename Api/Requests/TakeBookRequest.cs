using System.ComponentModel.DataAnnotations;

namespace Api.Requests;

public class TakeBookRequest
{
    [Required]
    public long BookCopyId { get; set; }

    [Required]
    public string SсheduledReturnDate { get; set; }
}