using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MovieService.Domain.Entities;
using MovieService.Infrastructure.Data;
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
            _apiKey = config["TMDB_API_KEY"]
                      ?? Environment.GetEnvironmentVariable("TMDB_API_KEY")
                      ?? throw new InvalidOperationException("TMDB_API_KEY is missing.");
        }

        // =========================================
        // 🔥 SYNC TRENDING (NowPlaying / Upcoming)
        // =========================================
        public async Task SyncNowPlayingAndUpcomingAsync(int pages = 3)
        {
            for (int page = 1; page <= pages; page++)
            {
                var url =
                    $"https://api.themoviedb.org/3/trending/movie/week" +
                    $"?api_key={_apiKey}&language=vi-VN&page={page}";

                var response = await _http.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var results = doc.RootElement.GetProperty("results");

                foreach (var movieJson in results.EnumerateArray())
                {
                    int tmdbId = movieJson.GetProperty("id").GetInt32();

                    var detail = await GetFromTmdbAsync(tmdbId);
                    if (detail == null) continue;

                    var existing = await _db.MovieDetails
                        .FirstOrDefaultAsync(m => m.TmdbId == tmdbId);

                    if (existing != null)
                    {
                        existing.Title = detail.Title;
                        existing.ReleaseDate = detail.ReleaseDate;
                        existing.Overview = detail.Overview;
                        existing.PosterUrl = detail.PosterUrl;
                        existing.Country = detail.Country;
                        existing.SpokenLanguages = detail.SpokenLanguages;
                        existing.Genres = detail.Genres;
                        existing.Crew = detail.Crew;
                        existing.Cast = detail.Cast;
                        existing.Trailer = detail.Trailer;
                        existing.Time = detail.Time;
                        existing.Age = detail.Age;
                        existing.Status = detail.Status;
                    }
                    else
                    {
                        _db.MovieDetails.Add(detail);
                    }
                }

                await _db.SaveChangesAsync();
            }
        }

        // =========================================
        // 🎬 GET MOVIE DETAIL (FULL + AGE)
        // =========================================
        public async Task<MovieDetail?> GetFromTmdbAsync(int tmdbId)
        {
            var movieUrl =
                $"https://api.themoviedb.org/3/movie/{tmdbId}" +
                $"?api_key={_apiKey}&language=vi-VN&append_to_response=videos,credits";

            var response = await _http.GetAsync(movieUrl);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            // ---------- basic ----------
            var title = root.GetProperty("title").GetString() ?? "";
            var overview = root.GetProperty("overview").GetString() ?? "";

            var releaseDateStr = root.TryGetProperty("release_date", out var rd)
                ? rd.GetString()
                : "";

            DateTime? releaseDate = null;
            if (DateTime.TryParse(releaseDateStr, out var parsed))
                releaseDate = parsed;

            // ---------- STATUS ----------
            string status =
                releaseDate.HasValue && releaseDate.Value.Date > DateTime.UtcNow.Date
                    ? "Upcoming"
                    : "NowPlaying";

            // ---------- poster ----------
            var posterUrl = root.TryGetProperty("poster_path", out var path)
                ? $"https://image.tmdb.org/t/p/w500{path.GetString()}"
                : "";

            // ---------- spoken languages ----------
            string spokenLanguages = "";
            if (root.TryGetProperty("spoken_languages", out var langs))
            {
                spokenLanguages = string.Join(", ",
                    langs.EnumerateArray()
                        .Select(l => l.GetProperty("english_name").GetString()));
            }

            // ---------- genres ----------
            string genres = "";
            if (root.TryGetProperty("genres", out var genreArr))
            {
                genres = string.Join(", ",
                    genreArr.EnumerateArray()
                        .Select(g => g.GetProperty("name").GetString()));
            }

            // ---------- cast & crew ----------
            string cast = "", crew = "";
            if (root.TryGetProperty("credits", out var credits))
            {
                if (credits.TryGetProperty("cast", out var castArr))
                {
                    cast = string.Join(", ",
                        castArr.EnumerateArray()
                            .Take(5)
                            .Select(c => c.GetProperty("name").GetString()));
                }

                if (credits.TryGetProperty("crew", out var crewArr))
                {
                    var director = crewArr.EnumerateArray()
                        .FirstOrDefault(c =>
                            c.TryGetProperty("job", out var job) &&
                            job.GetString() == "Director");

                    if (director.ValueKind != JsonValueKind.Undefined)
                        crew = director.GetProperty("name").GetString() ?? "";
                }
            }

            // ---------- trailer ----------
            string trailer = "";
            if (root.TryGetProperty("videos", out var videos) &&
                videos.TryGetProperty("results", out var vids))
            {
                var trailerObj = vids.EnumerateArray()
                    .FirstOrDefault(v =>
                        v.GetProperty("type").GetString() == "Trailer" &&
                        v.GetProperty("site").GetString() == "YouTube");

                if (trailerObj.ValueKind != JsonValueKind.Undefined)
                {
                    trailer =
                        $"https://www.youtube.com/watch?v={trailerObj.GetProperty("key").GetString()}";
                }
            }

            // ---------- runtime ----------
            int runtime = root.TryGetProperty("runtime", out var rt) && rt.ValueKind == JsonValueKind.Number
                ? rt.GetInt32()
                : 0;

            // ---------- AGE / CERTIFICATION ----------
            var ratingUrl =
                $"https://api.themoviedb.org/3/movie/{tmdbId}/release_dates?api_key={_apiKey}";

            var ratingResponse = await _http.GetAsync(ratingUrl);
            ratingResponse.EnsureSuccessStatusCode();

            var ratingJson = await ratingResponse.Content.ReadAsStringAsync();
            using var ratingDoc = JsonDocument.Parse(ratingJson);
            var ratingRoot = ratingDoc.RootElement;
            string? certification = null;

            // 1️⃣ Ưu tiên US
            certification = GetCertificationByCountry(ratingRoot, "US");

            // 2️⃣ Nếu không có → thử VN
            if (string.IsNullOrWhiteSpace(certification))
            {
                certification = GetCertificationByCountry(ratingRoot, "VN");
            }

            // 3️⃣ Nếu vẫn không có → lấy cái đầu tiên không rỗng
            if (string.IsNullOrWhiteSpace(certification))
            {
                foreach (var country in ratingRoot.GetProperty("results").EnumerateArray())
                {
                    if (!country.TryGetProperty("release_dates", out var releases))
                        continue;

                    foreach (var r in releases.EnumerateArray())
                    {
                        var cert = r.GetProperty("certification").GetString();
                        if (!string.IsNullOrWhiteSpace(cert))
                        {
                            certification = cert;
                            break;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(certification))
                        break;
                }
            }

            // 4️⃣ Map sang tuổi
            int age = ConvertRatingToAge(certification) ?? GuessAgeByGenres(genres);


            // ---------- CREATE ----------
            return new MovieDetail
            {
                TmdbId = tmdbId,
                Title = title,
                ReleaseDate = releaseDateStr ?? "",
                Overview = overview,
                PosterUrl = posterUrl,
                Country = "VN",
                SpokenLanguages = spokenLanguages,
                Genres = genres,
                Crew = crew,
                Cast = cast,
                Trailer = trailer,
                Time = runtime,
                Age = age,
                Status = status
            };
        }

        // =========================================
        // 🔢 RATING → AGE
        // =========================================
        private int? ConvertRatingToAge(string? rating)
        {
            if (string.IsNullOrWhiteSpace(rating))
                return null;

            return rating.ToUpper() switch
            {
                "G" => 0,
                "PG" => 10,
                "PG-13" => 13,
                "R" => 17,
                "NC-17" => 18,

                "P" => 0,
                "C13" => 13,
                "C16" => 16,
                "C18" => 18,

                _ => null
            };
        }
        private string? GetCertificationByCountry(JsonElement ratingRoot, string countryCode)
        {
            foreach (var country in ratingRoot.GetProperty("results").EnumerateArray())
            {
                if (country.GetProperty("iso_3166_1").GetString() != countryCode)
                    continue;

                if (!country.TryGetProperty("release_dates", out var releases))
                    continue;

                foreach (var r in releases.EnumerateArray())
                {
                    var cert = r.GetProperty("certification").GetString();
                    if (!string.IsNullOrWhiteSpace(cert))
                        return cert;
                }
            }
            return null;
        }

        private int GuessAgeByGenres(string genres)
        {
            if (genres.Contains("Horror", StringComparison.OrdinalIgnoreCase))
                return 18;
            if (genres.Contains("Action", StringComparison.OrdinalIgnoreCase))
                return 16;
            if (genres.Contains("Thriller", StringComparison.OrdinalIgnoreCase))
                return 16;

            return 13; // default an toàn
        }

    }
}