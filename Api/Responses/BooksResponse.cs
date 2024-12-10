using Core.Entities;

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

    public string AuthorFullName { get; set; }

    public string ArtworkUrl { get; set; }

    public int NumberOfCopies { get; set; }
}