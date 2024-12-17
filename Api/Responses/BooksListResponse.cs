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

    public List<AuthorListItem> Authors { get; set; }

    public string ArtworkUrl { get; set; }
}

public class AuthorListItem
{
    public long Id { get; set; }
    public string FullName { get; set; }
}