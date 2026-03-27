namespace Core.Entities;

public class BookFeedback
{
    public long Id { get; set; }

    public long TenantId { get; set; }
    
    public long BookId { get; set; }

    public Book Book { get; set; }

    public long EmployeeId { get; set; }

    public DateTime LeftFeedbackAtUtc { get; set; }

    public ProgressOfReading ProgressOfReading { get; set; }

    public int Rating { get; set; }

    public string? Advantages { get; set; }

    public string? Disadvantages { get; set; }
}
