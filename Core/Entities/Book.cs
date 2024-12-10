namespace Core.Entities
{
    public class Book
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public string Annotation { get; set; }

        public Language Language { get; set; }

        public List<Author> Authors { get; set; } 

        public DateTime CreatedAtUtc { get; set; }
        public DateTime? DeletedAtUtc { get; set; } = null;

        public string ArtworkUrl { get; set; }

        public int NumberOfCopies { get; set; }


        public Book(string title, 
            string annotation, 
            Language language,
            List<Author> authors, 
            string artworkUrl, 
            int numberOfCopies)
        {
            Title = title;
            Annotation = annotation;
            Language = language;
            Authors = authors;
            ArtworkUrl = artworkUrl;
            NumberOfCopies = numberOfCopies;
            CreatedAtUtc = DateTime.UtcNow;
        }

        public Book()
        {
        }
    }
}