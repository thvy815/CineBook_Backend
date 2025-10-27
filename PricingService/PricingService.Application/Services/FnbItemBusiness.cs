using PricingService.Domain.DTOs;
using PricingService.Domain.Entities;
using PricingService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricingService.Application.Services
{
    public class FnbItemBusiness
    {
        private readonly IFnbItemRepository _repository;

        public FnbItemBusiness(IFnbItemRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<FnbItemDTOs>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => new FnbItemDTOs
            {
                Id = e.id,
                Name = e.Name,
                Description = e.Description,
                UnitPrice = e.UnitPrice
            });
        }

        public async Task<FnbItemDTOs?> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            return new FnbItemDTOs
            {
                Id = entity.id,
                Name = entity.Name,
                Description = entity.Description,
                UnitPrice = entity.UnitPrice
            };
        }
        public async Task AddAsync(FnbItemDTOs dto)
        {
            var entity = new FnbItem
            {
                id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                UnitPrice = dto.UnitPrice
            };

            await _repository.AddAsync(entity);
        }

        public async Task UpdateAsync(FnbItemDTOs dto)
        {
            // Lấy entity cũ
            var entity = await _repository.GetByIdAsync(dto.Id);
            if (entity == null)
                throw new KeyNotFoundException("Item not found");

            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.UnitPrice = dto.UnitPrice;

            // Update thông qua repository
            await _repository.UpdateAsync(entity);  // repository sẽ gọi SaveChangesAsync bên trong
        }

        public Task DeleteAsync(Guid id) => _repository.DeleteAsync(id);

    }
}
