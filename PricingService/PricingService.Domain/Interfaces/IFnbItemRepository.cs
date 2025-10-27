using PricingService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricingService.Domain.Interfaces
{
    public interface IFnbItemRepository
    {
        Task<IEnumerable<FnbItem>> GetAllAsync();
        Task<FnbItem?> GetByIdAsync(Guid id);
        Task AddAsync(FnbItem fnbItem);
        Task UpdateAsync(FnbItem fnbItem);
        Task DeleteAsync(Guid id);
    }
}
