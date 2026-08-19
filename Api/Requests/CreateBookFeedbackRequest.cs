using System.ComponentModel.DataAnnotations;

namespace Api.Requests;

public class CreateBookFeedbackRequest
{
  [Required]
  public long BookId { get; set; }

  [Required]
  public string ProgressOfReading { get; set; }
  
  [Range(1, 5)]
  public int Rating { get; set; }

  public string? Advantages { get; set; }

  public string? Disadvantages { get; set; }
}
