using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowtimeService.Application.DTOs
{
    public class MovieFromMovieServiceDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public int Duration { get; set; }
        public string PosterUrl { get; set; } = string.Empty;
    }

}
