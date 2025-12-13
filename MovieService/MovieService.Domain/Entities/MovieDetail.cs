namespace MovieService.Domain.Entities
{
    public class MovieDetail
    {
        public Guid Id { get; set; }
        public int TmdbId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int? Age { get; set; }
        public string Status { get; set; } = string.Empty;
        public string SpokenLanguages { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public int? Time { get; set; }
        public string Genres { get; set; } = string.Empty;
        public string Crew { get; set; } = string.Empty;
        public string Cast { get; set; } = string.Empty;
        public string ReleaseDate { get; set; } = string.Empty;
        public string Overview { get; set; } = string.Empty;
        public string PosterUrl { get; set; } = string.Empty;
        public string Trailer { get; set; } = string.Empty;
    }
}
