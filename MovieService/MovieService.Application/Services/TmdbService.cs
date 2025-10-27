using System.Net.Http;
using System.Text.Json;
using MovieService.Domain.Entities;
using MovieService.Infrastructure.Data;
using Microsoft.Extensions.Configuration;

namespace MovieService.Application.Services
{
    public class TmdbService
    {
        private readonly HttpClient _http;
        private readonly MovieDbContext _db;
        private readonly string _apiKey;

        public TmdbService(HttpClient http, MovieDbContext db, IConfiguration config)
        {
            _http = http;
            _db = db;
            _apiKey = config["TMDB_API_KEY"] ?? throw new InvalidOperationException("TMDB_API_KEY is missing.");
        }

        public async Task<MovieDetail> GetFromTmdbAsync(int tmdbId)
        {
            var url = $"https://api.themoviedb.org/3/movie/{tmdbId}?api_key={_apiKey}&append_to_response=videos,credits";
            var response = await _http.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var movie = new MovieDetail
            {
                TmdbId = tmdbId,
                Title = root.GetProperty("title").GetString() ?? "",
                ReleaseDate = root.TryGetProperty("release_date", out var date) ? date.GetString() ?? "" : "",
                Overview = root.TryGetProperty("overview", out var overview) ? overview.GetString() ?? "" : "",
                PosterUrl = root.TryGetProperty("poster_path", out var path) ? $"https://image.tmdb.org/t/p/w500{path.GetString()}" : "",
                Status = root.TryGetProperty("status", out var status) ? status.GetString() ?? "" : "",
                Country = root.TryGetProperty("original_language", out var lang) ? lang.GetString() ?? "" : "",
                Age = null // cho phép null
            };

            _db.MovieDetails.Add(movie);
            await _db.SaveChangesAsync();

            return movie;
        }
    }
}
