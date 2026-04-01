using System.ComponentModel.DataAnnotations;

namespace Api.Requests;

public class ReturnBookRequest
{
  [Required]
  public long BookCopyId { get; set; }

  [Required]
  public string ProgressOfReading { get; set; }

  public int? Rating { get; set; }

  public string? Advantages { get; set; }

  public string? Disadvantages { get; set; }
}
