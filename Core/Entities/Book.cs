namespace Core.Entities
{
    public class Book
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public string Annotation { get; set; }

        public long AuthorId { get; set; }

        public Author Author { get; set; }

        public long LanguageId { get; set; }
        public Language Language { get; set; }

        public DateTime CreatedAtUtc { get; set; }
        public DateTime? DeletedAtUtc { get; set; } = null;

        public string ArtworkUrl { get; set; }

        public int NumberOfCopies { get; set; }

        public long StatusId { get; set; }

        public Status Status { get; set; }


        public Book(string title, 
            string annotation, 
            long authorId, 
            long languageId,
            string artworkUrl, 
            int numberOfCopies)
        {
            Title = title;
            Annotation = annotation;
            AuthorId = authorId;
            LanguageId = languageId;
            ArtworkUrl = artworkUrl;
            NumberOfCopies = numberOfCopies;
            StatusId = 1L;
            CreatedAtUtc = DateTime.UtcNow;
        }

        public Book()
        {
        }
    }
}