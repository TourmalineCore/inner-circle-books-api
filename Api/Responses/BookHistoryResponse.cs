namespace Api.Responses;

public class BookHistoryResponse
{
    public List<BookHistoryItem> List { get; set; }
    
    public long TotalCount { get; set; }
}

public class BookHistoryItem
{
    public long Id { get; set; }

    public long BookCopyId { get; set; }

    public string EmployeeFullName { get; set; }

    public string TakenDate { get; set; }

    public string ScheduledReturnDate { get; set; }

    public string? ActualReturnedDate { get; set; } = null;

    public string? ProgressOfReading { get; set; } = null;
}