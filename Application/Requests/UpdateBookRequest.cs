using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Requests
{
    public class UpdateBookRequest
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public string Annotation { get; set; }

        public string ArtworkUrl { get; set; }

    }
}
