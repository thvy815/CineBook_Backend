using BookingService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingService.Domain.Interfaces
{
    public interface IUsedPromotionRepository
    {
        Task<IEnumerable<UsedPromotion>> GetAllAsync();
        Task<UsedPromotion?> GetByIdAsync(Guid id);
        Task AddAsync(UsedPromotion usedPromotion);
        Task UpdateAsync(UsedPromotion usedPromotion);
        Task DeleteAsync(Guid id);
    }
}
