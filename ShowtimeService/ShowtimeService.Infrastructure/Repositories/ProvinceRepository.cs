using Microsoft.EntityFrameworkCore;
using ShowtimeService.Domain.Entities;
using ShowtimeService.Domain.Interfaces;
using ShowtimeService.Infrastructure.Data;
using System.Threading.Tasks;

namespace ShowtimeService.Infrastructure.Repositories
{
    public class ProvinceRepository : GenericRepository<Province>, IProvinceRepository
    {
        public ProvinceRepository(ShowtimeDbContext context) : base(context) { }

        public async Task<Province> GetByNameAsync(string name)
        {
            return await _context.Provinces.FirstOrDefaultAsync(p => p.Name == name);
        }
    }
}
