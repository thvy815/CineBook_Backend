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
            var random = new Random();
            const int startHour = 7;
            const int endHour = 22;

            // TimeZone VN
            var vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            foreach (var movie in movies)
            {
                for (int d = 0; d < 5; d++)
                {
                    var date = today.AddDays(d);

                    // Giờ bắt đầu max để phim kết thúc trước 22:00
                    int maxStartHour = endHour - (int)Math.Ceiling(movie.Duration / 60.0);
                    if (maxStartHour < startHour)
                        continue; // phim quá dài, bỏ qua

                    int hour = random.Next(startHour, maxStartHour + 1);
                    int minute = random.Next(0, 60);

                    // Giờ local VN với DateTimeKind.Unspecified
                    var startTimeLocal = DateTime.SpecifyKind(date.AddHours(hour).AddMinutes(minute), DateTimeKind.Unspecified);
                    var endTimeLocal = DateTime.SpecifyKind(startTimeLocal.AddMinutes(movie.Duration), DateTimeKind.Unspecified);

                    // Đảm bảo không vượt quá 22:00
                    if (endTimeLocal.Hour > endHour || (endTimeLocal.Hour == endHour && endTimeLocal.Minute > 0))
                        continue;

                    // Chuyển sang UTC để lưu DB
                    var startTimeUtc = TimeZoneInfo.ConvertTimeToUtc(startTimeLocal, vnTimeZone);
                    var endTimeUtc = TimeZoneInfo.ConvertTimeToUtc(endTimeLocal, vnTimeZone);

                    // Tạo showtime
                    var showtime = new Showtime
                    {
                        Id = Guid.NewGuid(),
                        RoomId = roomId,
                        TheaterId = theaterId,
                        MovieId = movie.Id,
                        StartTime = startTimeUtc,
                        EndTime = endTimeUtc
                    };

                    await _context.Showtimes.AddAsync(showtime);
                    await _context.SaveChangesAsync(); // cần lưu để có Id

                    // Tạo tất cả ghế cho showtime
                    await CreateShowtimeSeatsAsync(showtime.Id);
                }
            }
        }

        public async Task<int> CreateShowtimeSeatsAsync(Guid showtimeId)
        {
            // 1. Lấy showtime
            var showtime = await _context.Showtimes
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == showtimeId);

            if (showtime == null)
                throw new Exception("Showtime not found");

            // 2. Check đã tạo ghế cho showtime chưa
            bool existed = await _context.ShowtimeSeats
                .AnyAsync(x => x.ShowtimeId == showtimeId);

            if (existed)
                throw new Exception("Showtime seats already created");

            // 3. Lấy toàn bộ ghế của room
            var seats = await _context.Seats
                .Where(x => x.RoomId == showtime.RoomId)
                .ToListAsync();

            if (!seats.Any())
                throw new Exception("Room has no seats");

            // 4. Tạo ShowtimeSeat cho TẤT CẢ ghế
            var showtimeSeats = seats.Select(seat => new ShowtimeSeat
            {
                Id = Guid.NewGuid(),
                ShowtimeId = showtimeId,
                SeatId = seat.Id,          // 🎯 trỏ thẳng
                Status = "Available",
                UpdatedAt = DateTime.UtcNow
            }).ToList();

            await _context.ShowtimeSeats.AddRangeAsync(showtimeSeats);
            await _context.SaveChangesAsync();

            return showtimeSeats.Count;
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
            string date)
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
                        ShowtimeId = x.s.Id,
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
