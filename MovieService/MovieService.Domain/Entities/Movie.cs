using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieService.Domain.Entities
{
    public class Movie
    {
        public int Id { get; set; }
        public int TmdbId { get; set; }
        public string Title { get; set; }
        public int Age { get; set; }
        public string Status { get; set; }
        public string SpokenLanguages { get; set; }
        public string Country { get; set; }
        public int Time { get; set; }
        public string Genres { get; set; }
        public string Crew { get; set; }
        public string Cast { get; set; }
        public string ReleaseDate { get; set; }
        public string Overview { get; set; }
        public string PosterUrl { get; set; }
        public string Trailer { get; set; }
    }
}
