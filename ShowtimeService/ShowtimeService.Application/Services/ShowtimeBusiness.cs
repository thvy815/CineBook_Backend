using ShowtimeService.Application.DTOs;
using ShowtimeService.Domain.Entities;
using ShowtimeService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Xml.Serialization;
using ShowtimeService.Application.DTOs.ShowtimeService.Application.DTOs;

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

            if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out DateTime parsedDate))
            {
                var start = parsedDate.Date;
                var end = parsedDate.Date.AddDays(1);
                query = query.Where(s => s.StartTime >= start && s.StartTime < end);
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



    }
}
