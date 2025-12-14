using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieService.Domain.DTOs
{
    public class MovieSummaryResponse
    {
        public Guid Id { get; set; }
        public int? TmdbId { get; set; }
        public string Title { get; set; } = null!;
        public string PosterUrl { get; set; } = null!;
        public string Age { get; set; } = null!;
        public string Status { get; set; }
        public int? Time { get; set; }
        public List<string> SpokenLanguages { get; set; } = new();
        public List<string> Genres { get; set; } = new();
        public string Trailer { get; set; } = null!;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public double? Popularity { get; set; }
    }

    public class PagedResponse<T>
    {
        public List<T> Data { get; set; } = new();
        public int Page { get; set; }
        public int Size { get; set; }
        public long TotalElements { get; set; }
        public int TotalPages { get; set; }
    }
}
