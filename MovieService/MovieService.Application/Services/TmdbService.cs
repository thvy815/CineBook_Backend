using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MovieService.Domain.Entities;
using MovieService.Infrastructure.Data;
using System.Net.Http;
using System.Text.Json;

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
            _apiKey = config["TMDB_API_KEY"] ?? Environment.GetEnvironmentVariable("TMDB_API_KEY")
                      ?? throw new InvalidOperationException("TMDB_API_KEY is missing.");
        }

        // 🎬 Gọi TMDB API để lấy phim theo category (now_playing / upcoming)
        private async Task<int> SyncMoviesByCategoryAsync(string category, int totalPages = 2)
        {
            int imported = 0;

            for (int page = 1; page <= totalPages; page++)
            {
                var url = $"https://api.themoviedb.org/3/movie/{category}?api_key={_apiKey}&language=en-US&page={page}";
                var response = await _http.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var results = doc.RootElement.GetProperty("results");

                foreach (var movieJson in results.EnumerateArray())
                {
                    int tmdbId = movieJson.GetProperty("id").GetInt32();

                    // Nếu đã tồn tại thì chỉ update status
                    var existing = await _db.MovieDetails.FirstOrDefaultAsync(m => m.TmdbId == tmdbId);
                    if (existing != null)
                    {
                        existing.Status = category == "now_playing" ? "Now Playing" : "Upcoming";
                        continue;
                    }

                    // Gọi chi tiết để lấy thông tin và age
                    var detail = await GetFromTmdbAsync(tmdbId);
                    detail.Status = category == "now_playing" ? "Now Playing" : "Upcoming";
                    imported++;
                }

                await _db.SaveChangesAsync();
            }

            return imported;
        }

        // 🎬 Lấy chi tiết 1 phim và lưu vào DB
        public async Task<MovieDetail> GetFromTmdbAsync(int tmdbId)
        {
            var movieUrl = $"https://api.themoviedb.org/3/movie/{tmdbId}?api_key={_apiKey}&append_to_response=videos,credits";
            var response = await _http.GetAsync(movieUrl);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            // Lấy certification (rating)
            var ratingUrl = $"https://api.themoviedb.org/3/movie/{tmdbId}/release_dates?api_key={_apiKey}";
            var ratingResponse = await _http.GetAsync(ratingUrl);
            ratingResponse.EnsureSuccessStatusCode();

            var ratingJson = await ratingResponse.Content.ReadAsStringAsync();
            using var ratingDoc = JsonDocument.Parse(ratingJson);
            var ratingRoot = ratingDoc.RootElement;

            string? certification = null;
            foreach (var country in ratingRoot.GetProperty("results").EnumerateArray())
            {
                if (country.GetProperty("iso_3166_1").GetString() == "US")
                {
                    var releases = country.GetProperty("release_dates").EnumerateArray();
                    if (releases.Any())
                    {
                        certification = releases.First().GetProperty("certification").GetString();
                        break;
                    }
                }
            }

            // spoken_languages
            string spokenLanguages = root.TryGetProperty("spoken_languages", out var langs)
                ? string.Join(", ", langs.EnumerateArray().Select(l => l.GetProperty("english_name").GetString()))
                : "";

            // genres
            string genres = root.TryGetProperty("genres", out var genreArr)
                ? string.Join(", ", genreArr.EnumerateArray().Select(g => g.GetProperty("name").GetString()))
                : "";

            // credits (cast + crew)
            string cast = "", crew = "";
            if (root.TryGetProperty("credits", out var credits))
            {
                if (credits.TryGetProperty("cast", out var castArr))
                    cast = string.Join(", ", castArr.EnumerateArray().Take(5).Select(c => c.GetProperty("name").GetString()));

                if (credits.TryGetProperty("crew", out var crewArr))
                {
                    var director = crewArr.EnumerateArray().FirstOrDefault(c =>
                        c.TryGetProperty("job", out var job) && job.GetString() == "Director");
                    if (director.ValueKind != JsonValueKind.Undefined)
                        crew = director.GetProperty("name").GetString() ?? "";
                }
            }

            // videos (trailer)
            string trailer = "";
            if (root.TryGetProperty("videos", out var videos) && videos.TryGetProperty("results", out var vids))
            {
                var trailerObj = vids.EnumerateArray().FirstOrDefault(v =>
                    v.TryGetProperty("type", out var type) && type.GetString() == "Trailer" &&
                    v.TryGetProperty("site", out var site) && site.GetString() == "YouTube");
                if (trailerObj.ValueKind != JsonValueKind.Undefined)
                    trailer = $"https://www.youtube.com/watch?v={trailerObj.GetProperty("key").GetString()}";
            }

            // runtime
            int? runtime = root.TryGetProperty("runtime", out var rt) && rt.ValueKind == JsonValueKind.Number ? rt.GetInt32() : null;

            // Xóa bản cũ nếu có
            var existing = await _db.MovieDetails.FirstOrDefaultAsync(m => m.TmdbId == tmdbId);
            if (existing != null)
            {
                _db.MovieDetails.Remove(existing);
                await _db.SaveChangesAsync();
            }

            // Tạo phim mới
            var movie = new MovieDetail
            {
                TmdbId = tmdbId,
                Title = root.GetProperty("title").GetString() ?? "",
                ReleaseDate = root.TryGetProperty("release_date", out var date) ? date.GetString() ?? "" : "",
                Overview = root.TryGetProperty("overview", out var overview) ? overview.GetString() ?? "" : "",
                PosterUrl = root.TryGetProperty("poster_path", out var path) ? $"https://image.tmdb.org/t/p/w500{path.GetString()}" : "",
                Country = root.TryGetProperty("original_language", out var lang) ? lang.GetString() ?? "" : "",
                SpokenLanguages = spokenLanguages,
                Genres = genres,
                Crew = crew,
                Cast = cast,
                Trailer = trailer,
                Time = runtime ?? 0,
                Age = ConvertRatingToAge(certification) ?? 0
            };

            _db.MovieDetails.Add(movie);
            await _db.SaveChangesAsync();

            return movie;
        }



        // 🌍 Hàm gọi đồng bộ toàn bộ (đang & sắp chiếu)
        public async Task SyncNowPlayingAndUpcomingAsync()
        {
            var baseUrl = "https://api.themoviedb.org/3/movie";
            var urls = new Dictionary<string, string>
    {
        { "Now Playing", $"{baseUrl}/now_playing?api_key={_apiKey}&language=en-US&page=1" },
        { "Upcoming", $"{baseUrl}/upcoming?api_key={_apiKey}&language=en-US&page=1" }
    };

            foreach (var (status, url) in urls)
            {
                var response = await _http.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var results = doc.RootElement.GetProperty("results");

                int count = 0;
                foreach (var movie in results.EnumerateArray().Take(40))
                {
                    int tmdbId = movie.GetProperty("id").GetInt32();
                    var detail = await GetFromTmdbAsync(tmdbId);
                    detail.Status = status;
                    _db.MovieDetails.Update(detail);
                    await _db.SaveChangesAsync();

                    count++;
                    Console.WriteLine($"✅ Synced {count}/40 {status}: {detail.Title}");
                }
            }
        }



        // 🔢 Chuyển rating → độ tuổi
        private int? ConvertRatingToAge(string? rating)
        {
            if (string.IsNullOrEmpty(rating))
                return null;

            return rating.ToUpper() switch
            {
                "G" => 0,
                "PG" => 10,
                "PG-13" => 13,
                "R" => 17,
                "NC-17" => 18,
                _ => null
            };
        }
    }
}
