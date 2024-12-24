namespace Api.Responses
{
    public class SingleBookResponse
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public string Annotation { get; set; }

        public string Language { get; set; }

        public List<AuthorResponse> Authors { get; set; }

        public string ArtworkUrl { get; set; }
    }
}