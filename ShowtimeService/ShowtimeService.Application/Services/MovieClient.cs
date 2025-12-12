using ShowtimeService.Application.DTOs;
using System.Net.Http.Json;

namespace ShowtimeService.Application.Services
{
    public class MovieClient
    {
        private readonly HttpClient _http;

        public MovieClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<MovieFromMovieServiceDto>> GetNowPlayingAsync()
        {
            var movies = await _http.GetFromJsonAsync<List<MovieFromMovieServiceDto>>(
                "/api/MovieDetail/status/now-playing"
            );

            return movies ?? new List<MovieFromMovieServiceDto>();
        }

        public async Task<List<MovieFromMovieServiceDto>> GetByIdsAsync(List<Guid> ids)
        {
            var url = "/api/MovieDetail/by-ids";
            var response = await _http.PostAsJsonAsync(url, ids);

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<MovieFromMovieServiceDto>>() ?? new();
        }
    }

}
