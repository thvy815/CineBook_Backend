using ShowtimeService.Application.DTOs;
using ShowtimeService.Domain.Entities;
using ShowtimeService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ShowtimeService.Application.Services
{
    public class ProvinceService
    {
        private readonly ShowtimeDbContext _context;

        public ProvinceService(ShowtimeDbContext context)
        {
            _context = context;
        }

        // ================= GET ALL =================
        public async Task<IEnumerable<ProvinceDto>> GetAllAsync()
        {
            return await _context.Provinces
                .Select(p => new ProvinceDto
                {
                    Id = p.Id,
                    Name = p.Name
                })
                .ToListAsync();
        }

        // ================= GET BY ID =================
        public async Task<ProvinceDto?> GetByIdAsync(Guid id)
        {
            var province = await _context.Provinces.FindAsync(id);
            if (province == null) return null;

            return new ProvinceDto
            {
                Id = province.Id,
                Name = province.Name
            };
        }

        // ================= CREATE =================
        public async Task<ProvinceDto> CreateAsync(CreateProvinceDto dto)
        {
            var entity = new Province
            {
                Id = Guid.NewGuid(),
                Name = dto.Name
            };

            _context.Provinces.Add(entity);
            await _context.SaveChangesAsync();

            return new ProvinceDto
            {
                Id = entity.Id,
                Name = entity.Name
            };
        }

        // ================= UPDATE =================
        public async Task<ProvinceDto?> UpdateAsync(Guid id, UpdateProvinceDto dto)
        {
            var entity = await _context.Provinces.FindAsync(id);
            if (entity == null) return null;

            entity.Name = dto.Name;

            await _context.SaveChangesAsync();

            return new ProvinceDto
            {
                Id = entity.Id,
                Name = entity.Name
            };
        }

        // ================= DELETE =================
        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _context.Provinces.FindAsync(id);
            if (entity == null) return false;

            _context.Provinces.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
