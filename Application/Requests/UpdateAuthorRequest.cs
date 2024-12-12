using Core.Entities;

namespace Application.Requests
{
    public class UpdateAuthorRequest
    {
        public long Id { get; set; }

        public string FullName { get; set; }

    }
}