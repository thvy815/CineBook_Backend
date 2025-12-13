using ShowtimeService.Application.DTOs;
using ShowtimeService.Domain.Entities;
using ShowtimeService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Xml.Serialization;
using System.Globalization;

namespace ShowtimeService.Application.Services
{
    public class ShowtimeBusiness
    {
        private readonly ShowtimeDbContext _context;
        private readonly MovieClient _movieClient;
        public ShowtimeBusiness(ShowtimeDbContext context, MovieClient movieClient)
        {
            _context = context;
            _movieClient = movieClient;  // Gán đúng DI vào
        }

        // Lấy tất cả suất chiếu
        public async Task<IEnumerable<ShowtimeDto>> GetAllAsync()
        {
            var showtimes = await _context.Showtimes
                .Include(s => s.Theater)
                .Include(s => s.Room)
                .ToListAsync();

            // Lấy toàn bộ MovieId trong danh sách showtimes
            var movieIds = showtimes.Select(s => s.MovieId).Distinct().ToList();

            // Gọi MovieService chỉ 1 lần
            var movies = await _movieClient.GetByIdsAsync(movieIds);

            var result = showtimes.Select(st =>
            {
                var movie = movies.FirstOrDefault(m => m.Id == st.MovieId);

                return new ShowtimeDto
                {
                    Id = st.Id,

                    MovieId = st.MovieId,
                    MovieTitle = movie?.Title ?? "",
                    PosterUrl = movie?.PosterUrl ?? "",

                    TheaterId = st.TheaterId,
                    TheaterName = st.Theater?.Name ?? "",
                    TheaterAddress = st.Theater?.Address ?? "",

                    RoomId = st.RoomId,

                    StartTime = st.StartTime,
                    StartTimeFormatted = st.StartTime.ToLocalTime().ToString("HH:mm"),

                    Date = st.StartTime.ToString("yyyy-MM-dd")
                };
            });

            return result;
        }



        // Lấy suất chiếu theo ID
        public async Task<ShowtimeDto?> GetByIdAsync(Guid id)
        {
            var showtime = await _context.Showtimes.FindAsync(id);
            if (showtime == null) return null;

            return new ShowtimeDto
            {
                Id = showtime.Id,
                MovieId = showtime.MovieId,
                TheaterId = showtime.TheaterId,
                RoomId = showtime.RoomId,
                StartTime = showtime.StartTime
            };
        }

        // Tạo mới suất chiếu
        public async Task<ShowtimeDto> CreateAsync(CreateShowtimeDto dto)
        {
            var entity = new Showtime
            {
                Id = Guid.NewGuid(),
                MovieId = dto.MovieId,
                TheaterId = dto.TheaterId,
                RoomId = dto.RoomId,
                StartTime = DateTime.SpecifyKind(dto.StartTime, DateTimeKind.Utc),
                EndTime = DateTime.SpecifyKind(dto.EndTime, DateTimeKind.Utc)
            };

            _context.Showtimes.Add(entity);
            await _context.SaveChangesAsync();

            return new ShowtimeDto
            {
                Id = entity.Id,
                MovieId = entity.MovieId,
                TheaterId = entity.TheaterId,
                RoomId = entity.RoomId,
                StartTime = entity.StartTime
            };
        }

        // Cập nhật suất chiếu
        public async Task<ShowtimeDto?> UpdateAsync(Guid id, CreateShowtimeDto dto)
        {
            var entity = await _context.Showtimes.FindAsync(id);
            if (entity == null) return null;

            entity.MovieId = dto.MovieId;
            entity.TheaterId = dto.TheaterId;
            entity.RoomId = dto.RoomId;
            entity.StartTime = dto.StartTime;
            entity.EndTime = dto.EndTime;

            await _context.SaveChangesAsync();

            return new ShowtimeDto
            {
                Id = entity.Id,
                MovieId = entity.MovieId,
                TheaterId = entity.TheaterId,
                RoomId = entity.RoomId,
                StartTime = entity.StartTime
            };
        }

        // Xóa suất chiếu
        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _context.Showtimes.FindAsync(id);
            if (entity == null) return false;

            _context.Showtimes.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task GenerateAutoShowtimesAsync(Guid theaterId, Guid roomId)
        {
            var movies = await _movieClient.GetNowPlayingAsync();

            if (!movies.Any())
                throw new Exception("Movie Service did not return any NOW_PLAYING movies.");

            var today = DateTime.Today;

            var timeSlots = new[]
            {
                new TimeSpan(10, 0, 0),
                new TimeSpan(14, 0, 0),
                new TimeSpan(19, 0, 0)
            };

            foreach (var movie in movies)
            {
                for (int d = 0; d < 5; d++)
                {
                    var date = today.AddDays(d);

                    foreach (var slot in timeSlots)
                    {
                        var startTime = date.Add(slot);
                        var endTime = startTime.AddMinutes(movie.Duration);
                        var showtime = new Showtime
                        {
                            RoomId = roomId,
                            TheaterId = theaterId,
                            StartTime = startTime.ToUniversalTime(),
                            EndTime = endTime.ToUniversalTime(),
                            MovieId = movie.Id
                        };

                        await _context.Showtimes.AddAsync(showtime);
                        await _context.SaveChangesAsync();

                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        // Lọc suất chiếu theo theaterId, movieId, date
        public async Task<IEnumerable<ShowtimeDto>> FilterAsync(Guid? theaterId, Guid? movieId, string? date)
        {
            var query = _context.Showtimes
                .Include(s => s.Theater)
                .Include(s => s.Room)
                .AsQueryable();

            if (theaterId.HasValue && theaterId.Value != Guid.Empty)
                query = query.Where(s => s.TheaterId == theaterId.Value);

            if (movieId.HasValue && movieId.Value != Guid.Empty)
                query = query.Where(s => s.MovieId == movieId.Value);


            if (!string.IsNullOrEmpty(date))
            {
                // Định dạng ngày phổ biến
                string[] formats = new string[]
                {
                    "dd/MM/yyyy", "d/M/yyyy",     // 13/12/2025, 1/1/2025
                    "dd-MM-yyyy", "d-M-yyyy",     // 13-12-2025
                    "MM/dd/yyyy", "M/d/yyyy",     // 12/13/2025
                    "MM-dd-yyyy", "M-d-yyyy"      // 12-13-2025
                        };

                if (DateTime.TryParseExact(date, formats,
                                           System.Globalization.CultureInfo.InvariantCulture,
                                           System.Globalization.DateTimeStyles.None,
                                           out DateTime parsedDate))
                {
                    var start = DateTime.SpecifyKind(parsedDate.Date, DateTimeKind.Utc);
                    var end = parsedDate.Date.AddDays(1);

                    // Lọc tất cả showtimes có StartTime trong ngày đó
                    query = query.Where(s => s.StartTime >= start && s.StartTime < end);
                }
            }

                var showtimes = await query.ToListAsync();

            var movieIds = showtimes.Select(s => s.MovieId).Distinct().ToList();
            List<MovieFromMovieServiceDto> movies;
            if (movieIds.Any())
            {
                movies = await _movieClient.GetByIdsAsync(movieIds);
            }
            else
            {
                movies = new List<MovieFromMovieServiceDto>();
            }

            var result = showtimes.Select(st =>
            {
                var movie = movies.FirstOrDefault(m => m.Id == st.MovieId);

                return new ShowtimeDto
                {
                    Id = st.Id,
                    MovieId = st.MovieId,
                    MovieTitle = movie?.Title ?? "",
                    PosterUrl = movie?.PosterUrl ?? "",
                    TheaterId = st.TheaterId,
                    TheaterName = st.Theater?.Name ?? "",
                    TheaterAddress = st.Theater?.Address ?? "",
                    RoomId = st.RoomId,
                    StartTime = st.StartTime,
                    StartTimeFormatted = st.StartTime.ToLocalTime().ToString("HH:mm"),
                    Date = st.StartTime.ToString("yyyy-MM-dd")
                };
            });

            return result;
        }

        public async Task<IEnumerable<ShowtimeDto>> FilterShowtimesAsync(
               Guid? theaterId,
               Guid? movieId,
               string date)
        {
            var query = _context.Showtimes.AsNoTracking().AsQueryable();

            if (theaterId.HasValue)
                query = query.Where(s => s.TheaterId == theaterId.Value);

            if (movieId.HasValue)
                query = query.Where(s => s.MovieId == movieId.Value);

            if (!string.IsNullOrEmpty(date))
            {
                string[] formats = new string[]
                {
                "dd/MM/yyyy", "d/M/yyyy",
                "dd-MM-yyyy", "d-M-yyyy",
                "MM/dd/yyyy", "M/d/yyyy",
                "MM-dd-yyyy", "M-d-yyyy",
                "yyyy/MM/dd", "yyyy-M-d", "yyyy-MM-dd"
                };

                if (!DateTime.TryParseExact(date, formats, CultureInfo.InvariantCulture,
                                            DateTimeStyles.None, out DateTime parsedDate))
                    throw new ArgumentException("Định dạng ngày không hợp lệ");

                var vnZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                var startUtc = TimeZoneInfo.ConvertTimeToUtc(parsedDate.Date, vnZone);
                var endUtc = TimeZoneInfo.ConvertTimeToUtc(parsedDate.Date.AddDays(1), vnZone);

                query = query.Where(s => s.StartTime >= startUtc && s.StartTime < endUtc);
            }

            // Join với Movie, Theater, Room
            var showtimes = await query
                .Join(_context.Theaters,
                      s => s.TheaterId,
                      t => t.Id,
                      (s, t) => new { s, t })
                .Join(_context.Rooms,
                      st => st.s.RoomId,
                      r => r.Id,
                      (st, r) => new
                      {
                          Showtime = st.s,
                          Theater = st.t,
                          Room = r
                      })
                .OrderBy(x => x.Showtime.StartTime)
                .ToListAsync();
            var movieIds = showtimes.Select(x => x.Showtime.MovieId).Distinct().ToList();
            var movies = await _movieClient.GetByIdsAsync(movieIds);
            var result = showtimes.Select(x =>
            {
                var movie = movies.FirstOrDefault(m => m.Id == x.Showtime.MovieId);

                return new ShowtimeDto
                {
                    Id = x.Showtime.Id,
                    MovieId = x.Showtime.MovieId,
                    MovieTitle = movie?.Title ?? "",
                    PosterUrl = movie?.PosterUrl ?? "",
                    TheaterId = x.Theater.Id,
                    TheaterName = x.Theater.Name,
                    TheaterAddress = x.Theater.Address,
                    RoomId = x.Room.Id,
                    RoomName = x.Room.Name,

                    StartTime = x.Showtime.StartTime,
                    StartTimeFormatted = x.Showtime.StartTime.ToString("HH:mm"),
                    Date = x.Showtime.StartTime.ToString("yyyy-MM-dd")
                };
            });
            return result;
        }

        private (DateTime startUtc, DateTime endUtc) ParseDate(string date)
        {
            string[] formats =
            {
        "dd/MM/yyyy", "d/M/yyyy",
        "dd-MM-yyyy", "d-M-yyyy",
        "MM/dd/yyyy", "M/d/yyyy",
        "MM-dd-yyyy", "M-d-yyyy",
        "yyyy-MM-dd"
    };

            if (!DateTime.TryParseExact(
                    date,
                    formats,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var parsed))
                throw new ArgumentException("Ngày không hợp lệ");

            var vnZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            var startUtc = TimeZoneInfo.ConvertTimeToUtc(parsed.Date, vnZone);
            var endUtc = TimeZoneInfo.ConvertTimeToUtc(parsed.Date.AddDays(1), vnZone);

            return (startUtc, endUtc);
        }

        public async Task<List<TheaterShowtimeDto>> FilterShowtimeAsync(
            Guid provinceId,
            Guid movieId,
            string date,
            Guid? theaterId = null)
        {
            var (startUtc, endUtc) = ParseDate(date);


            // 1. Query showtime + theater
            var query =
                from s in _context.Showtimes.AsNoTracking()
                join t in _context.Theaters on s.TheaterId equals t.Id
                join r in _context.Rooms on s.RoomId equals r.Id
                where
                    s.MovieId == movieId &&
                    t.ProvinceId == provinceId &&
                    s.StartTime >= startUtc &&
                    s.StartTime < endUtc
                select new { s, t, r };


            if (theaterId.HasValue)
                query = query.Where(x => x.t.Id == theaterId.Value);

            var raw = await query
                .OrderBy(x => x.s.StartTime)
                .ToListAsync();

            // 2. Group theo rạp
            var result = raw
                .GroupBy(x => new
                {
                    x.t.Id,
                    x.t.Name,
                    x.t.Address
                })
                .Select(g => new TheaterShowtimeDto
                {
                    TheaterId = g.Key.Id,
                    TheaterName = g.Key.Name,
                    TheaterAddress = g.Key.Address,
                    Showtimes = g.Select(x => new ShowtimeLiteDto
                    {
                        RoomId = x.s.RoomId,
                        RoomName = x.r.Name,

                        StartTimeFormatted = x.s.StartTime
                            .ToLocalTime()
                            .ToString("HH:mm"),
                        Date = x.s.StartTime
                            .ToLocalTime()
                            .ToString("yyyy-MM-dd")
                    }).ToList()
                })
                .ToList();

            return result;
        }



    }
}
