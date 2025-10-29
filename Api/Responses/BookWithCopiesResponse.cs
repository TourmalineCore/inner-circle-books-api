namespace Api.Responses;

public class BookWithCopiesResponse
{
    public string BookTitle { get; set; }
    public List<BookCopyResponse> BookCopies { get; set; }
}


public class BookCopyResponse
{
    public long BookCopyId { get; set; }

    public string SecretKey { get; set; }
}
