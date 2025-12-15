using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PricingService.Domain.Interfaces;


namespace PricingService.Infrastructure.Repositories
{
    public class FnbItemRepository : IFnbItemRepository
    {
        private readonly PricingDbContext _context;

        public FnbItemRepository(PricingDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FnbItem>> GetAllAsync() => await _context.FnbItems.ToListAsync();

        public async Task<FnbItem?> GetByIdAsync(Guid id) => await _context.FnbItems.FindAsync(id);

        public async Task AddAsync(FnbItem fnbItem)
        {
            _context.FnbItems.Add(fnbItem);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(FnbItem fnbItem)
        {
            _context.FnbItems.Update(fnbItem);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var fnbItem = await _context.FnbItems.FindAsync(id);
            if (fnbItem != null)
            {
                _context.FnbItems.Remove(fnbItem);
                await _context.SaveChangesAsync();
            }
        }
    }
}