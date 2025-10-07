using Core;

namespace Api.Responses;

public class SingleBookResponse
{
    public long Id { get; set; }

    public string Title { get; set; }

    public string Annotation { get; set; }

    public string Language { get; set; }

    public List<AuthorResponse> Authors { get; set; }

    public string CoverUrl { get; set; }

    public List<BookCopyResponse> BookCopies { get; set; }

    public List<EmployeeWhoReadsNow> EmployeesWhoReadNow { get; set; }
}

public class BookCopyResponse
{
    public long BookCopyId { get; set; }

    public int CopyNumber { get; set; }
}