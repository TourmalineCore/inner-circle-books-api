using System.ComponentModel.DataAnnotations;

namespace Api.Requests;

public class ReturnBookRequest
{
  [Required]
  public long BookCopyId { get; set; }

  [Required]
  public string ProgressOfReading { get; set; }
}
