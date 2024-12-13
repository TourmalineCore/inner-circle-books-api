namespace Api.Responses;

public class BooksResponse
{
    public List<Book> Books { get; set; }
}

public class Book
{
    public long Id { get; set; }

    public string Title { get; set; }

    public string Annotation { get; set; }

    public string Language { get; set; }

    public List<Author> Authors { get; set; }

    public string ArtworkUrl { get; set; }
}

public class Author
{
    public string FullName { get; set; }
}