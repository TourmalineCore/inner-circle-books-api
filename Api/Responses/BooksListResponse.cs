namespace Api.Responses;

public class BooksListResponse
{
    public List<BookListItem> Books { get; set; }
}

public class BookListItem
{
    public long Id { get; set; }

    public string Title { get; set; }

    public string Annotation { get; set; }

    public string Language { get; set; }

    public List<AuthorResponse> Authors { get; set; }

    public string BookCoverUrl { get; set; }
}