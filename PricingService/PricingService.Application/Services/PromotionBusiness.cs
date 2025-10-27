using Microsoft.EntityFrameworkCore;
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
    public class PromotionBusiness
    {
        private readonly IPromotionRepository _repository;

        public PromotionBusiness(IPromotionRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<PromotionDTOs>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => new PromotionDTOs
            {
                Id = e.Id,
                Code = e.Code,
                DiscountType = e.DiscountType,
                DiscountValue = e.DiscountValue,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                IsActive = e.IsActive,
                IsOneTimeUse = e.IsOneTimeUse,
                Description = e.Description
            });
        }

        public async Task<PromotionDTOs?> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            return new PromotionDTOs
            {
                Id = entity.Id,
                Code = entity.Code,
                DiscountType = entity.DiscountType,
                DiscountValue = entity.DiscountValue,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                IsActive = entity.IsActive,
                IsOneTimeUse = entity.IsOneTimeUse,
                Description = entity.Description
            };
        }

        public async Task AddAsync(PromotionDTOs dto)
        {
            var entity = new Promotion
            {
                Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
                Code = dto.Code,
                DiscountType = dto.DiscountType,
                DiscountValue = dto.DiscountValue,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsActive = dto.IsActive,
                IsOneTimeUse = dto.IsOneTimeUse,
                Description = dto.Description
            };

            await _repository.AddAsync(entity);
        }

        public async Task UpdateAsync(PromotionDTOs dto)
        {
            // Lấy entity cũ
            var entity = await _repository.GetByIdAsync(dto.Id);
            if (entity == null)
                throw new KeyNotFoundException("Promotion not found");

            // Cập nhật các trường
            entity.Code = dto.Code;
            entity.DiscountType = dto.DiscountType;
            entity.DiscountValue = dto.DiscountValue;
            entity.StartDate = dto.StartDate;
            entity.EndDate = dto.EndDate;
            entity.IsActive = dto.IsActive;
            entity.IsOneTimeUse = dto.IsOneTimeUse;
            entity.Description = dto.Description;

            // Update thông qua repository
            await _repository.UpdateAsync(entity);  // repository sẽ gọi SaveChangesAsync

        }

        public Task DeleteAsync(Guid id) => _repository.DeleteAsync(id);
    }
}
