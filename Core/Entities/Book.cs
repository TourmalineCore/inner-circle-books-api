namespace Core.Entities
{
    public class Book
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public string Annotation { get; set; }

        public Language Language { get; set; }

        public long AuthorId { get; set; }

        public Author Author { get; set; }

        public DateTime CreatedAtUtc { get; set; }
        public DateTime? DeletedAtUtc { get; set; } = null;

        public string ArtworkUrl { get; set; }

        public int NumberOfCopies { get; set; }


        public Book(string title, 
            string annotation, 
            Language language,
            long authorId, 
            string artworkUrl, 
            int numberOfCopies)
        {
            Title = title;
            Annotation = annotation;
            AuthorId = authorId;
            ArtworkUrl = artworkUrl;
            NumberOfCopies = numberOfCopies;
            CreatedAtUtc = DateTime.UtcNow;
        }

        public Book()
        {
        }
    }
}