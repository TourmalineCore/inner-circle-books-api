using Core.Entities;

namespace Application.Requests
{
    public class UpdateBookRequest
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public string Annotation { get; set; }

        public string Language { get; set; }

        public List<string> Authors { get; set; }

        public string ArtworkUrl { get; set; }

    }
}