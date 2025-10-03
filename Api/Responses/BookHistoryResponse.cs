namespace Api.Responses;

public class BookHistoryResponse
{
    public List<BookHistoryItem> BookHistory { get; set; }
}

public class BookHistoryItem
{
    public int CopyNumber { get; set; }

    public string EmployeeFullName { get; set; }

    public string TakenDate { get; set; }

    public string ScheduledReturnDate { get; set; }

    public string? ActualReturnedDate { get; set; } = null;

    public string? ProgressOfReading { get; set; } = null;
}