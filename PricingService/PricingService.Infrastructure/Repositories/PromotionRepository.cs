using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PricingService.Domain.Entities;
using PricingService.Domain.Interfaces;
using PricingService.Infrastructure.Data;

namespace PricingService.Infrastructure.Repositories
{
    public class PromotionRepository : IPromotionRepository
    {
        private readonly PricingDbContext _context;

        public PromotionRepository(PricingDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Promotion>> GetAllAsync() => await _context.Promotions.ToListAsync();

        public async Task<Promotion?> GetByIdAsync(Guid id) => await _context.Promotions.FindAsync(id);

        public async Task AddAsync(Promotion promotion)
        {
            _context.Promotions.Add(promotion);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Promotion promotion)
        {
            _context.Promotions.Update(promotion);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion != null)
            {
                _context.Promotions.Remove(promotion);
                await _context.SaveChangesAsync();
            }
        }
    }
}
