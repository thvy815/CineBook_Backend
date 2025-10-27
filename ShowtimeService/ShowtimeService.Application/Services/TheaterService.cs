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
    }
}
