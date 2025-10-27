using ShowtimeService.Domain.Entities;
using System.Threading.Tasks;

namespace ShowtimeService.Domain.Interfaces
{
    public interface IProvinceRepository : IGenericRepository<Province>
    {
        Task<Province> GetByNameAsync(string name);
    }
}
