using Core.Entities;

namespace Api.Responses;

public class BooksFeedbackListResponse
{
  public List<BookFeedbackDto> BookFeedbackList { get; set; }

  public int FeedbackCount { get; set; }
}

public class BookFeedbackDto
{
    public long Id { get; set; }

    public string EmployeeFullName { get; set; }

    public DateTime LeftFeedbackAtUtc { get; set; }

    public ProgressOfReading ProgressOfReading { get; set; }

    public int Rating { get; set; }

    public string? Advantages { get; set; }

    public string? Disadvantages { get; set; }
}
