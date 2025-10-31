namespace Core.Entities;

public class BookCopyReadingHistory
{
    public long Id { get; set; }

    public long TenantId { get; set; }

    public long BookCopyId { get; set; }

    public BookCopy BookCopy { get; set; }

    public long ReaderEmployeeId { get; set; }

    public DateTime TakenAtUtc { get; set; }

    public DateOnly ScheduledReturnDate { get; set; }

    public DateTime? ActualReturnedAtUtc { get; set; } = null;

    public ProgressOfReading? ProgressOfReading { get; set; } = null;
}