namespace Core.Entities;

public class BookCopy
{
    public long Id { get; set; }

    public long TenantId { get; set; }

    public long BookId { get; set; }

    public Book Book { get; set; }

    public List<BookCopyReadingHistory> ReadingHistoryList { get; set; }
}