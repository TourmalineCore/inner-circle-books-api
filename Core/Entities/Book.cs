namespace Core.Entities
{
    public class Book
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public string Annotation { get; set; }

        public long AuthorId { get; set; }

        public Author Author { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public string ArtworkUrl { get; set; }

        public int NumberOfCopies { get; set; }

        public long StatusId { get; set; }

        public Status Status { get; set; }

        public long CategoryId { get; set; }

        public Category Category { get; set; }

        public List<Tag> Tags { get; set; }

        public Book(string title, 
            string annotation, 
            long authorId, 
            string artworkUrl, 
            int numberOfCopies, 
            long statusId, 
            long categoryId, 
            List<Tag> tags)
        {
            Title = title;
            Annotation = annotation;
            AuthorId = authorId;
            ArtworkUrl = artworkUrl;
            NumberOfCopies = numberOfCopies;
            StatusId = statusId;
            CategoryId = categoryId;
            CreatedAtUtc = DateTime.UtcNow;
            Tags = tags;
        }

        public Book()
        {
        }
    }
}