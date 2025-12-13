using ShowtimeService.Application.DTOs;
using ShowtimeService.Domain.Entities;
using ShowtimeService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ShowtimeService.Application.Services
{
    public class TheaterService
    {
        private readonly ShowtimeDbContext _context;
        public TheaterService(ShowtimeDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TheaterDto>> GetAllAsync()
        {
            return await _context.Theaters
                .Select(t => new TheaterDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Address = t.Address,
                    Description = t.Description,
                    ProvinceId = t.ProvinceId,
                    Status = t.Status
                }).ToListAsync();
        }

        public async Task<TheaterDto> CreateAsync(CreateTheaterDto dto)
        {
            var entity = new Theater
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Address = dto.Address,
                Description = dto.Description,
                ProvinceId = dto.ProvinceId,
                Status = dto.Status
            };
            _context.Theaters.Add(entity);
            await _context.SaveChangesAsync();
            return new TheaterDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Address = entity.Address,
                Description = entity.Description,
                ProvinceId = entity.ProvinceId,
                Status = entity.Status
            };
        }

        public async Task<IEnumerable<TheaterDto>> GetByProvinceWithShowtimeAsync(Guid? provinceId, string date)
        {
            var theatersQuery = _context.Theaters.AsQueryable();

            if (provinceId.HasValue)
            {
                theatersQuery = theatersQuery.Where(t => t.ProvinceId == provinceId.Value);
            }

            if (!string.IsNullOrEmpty(date))
            {
                // Hỗ trợ nhiều định dạng ngày
                string[] formats = new string[]
                {
            "dd/MM/yyyy", "d/M/yyyy", "dd-MM-yyyy", "d-M-yyyy",
            "MM/dd/yyyy", "M/d/yyyy", "MM-dd-yyyy", "M-d-yyyy",
            "yyyy/MM/dd", "yyyy-M-d", "yyyy-MM-dd"
                };

                if (!DateTime.TryParseExact(date, formats, System.Globalization.CultureInfo.InvariantCulture,
                                            System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
                {
                    throw new ArgumentException("Định dạng ngày không hợp lệ");
                }

                // Giờ Việt Nam → UTC
                var vnZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                var startUtc = TimeZoneInfo.ConvertTimeToUtc(parsedDate.Date, vnZone);
                var endUtc = TimeZoneInfo.ConvertTimeToUtc(parsedDate.Date.AddDays(1), vnZone);

                // Lọc rạp có showtime trong ngày
                theatersQuery = theatersQuery
                    .Where(t => _context.Showtimes
                        .Any(s => s.TheaterId == t.Id && s.StartTime >= startUtc && s.StartTime < endUtc));
            }

            return await theatersQuery
                .Select(t => new TheaterDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Address = t.Address,
                    Description = t.Description,
                    ProvinceId = t.ProvinceId,
                    Status = t.Status
                })
                .ToListAsync();
        }



    }
}
