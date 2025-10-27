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
    public class SeatPriceBusiness
    {
        private readonly ISeatPriceRepository _repository;

        public SeatPriceBusiness(ISeatPriceRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<SeatPriceDTOs>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => new SeatPriceDTOs
            {
                Id = e.Id,
                SeatType = e.SeatType,
                TicketType = e.TicketType,
                BasePrice = e.BasePrice,
                Description = e.Description
            });
        }

        public async Task<SeatPriceDTOs?> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            return new SeatPriceDTOs
            {
                Id = entity.Id,
                SeatType = entity.SeatType,
                TicketType = entity.TicketType,
                BasePrice = entity.BasePrice,
                Description = entity.Description
            };
        }

        public async Task AddAsync(SeatPriceDTOs dto)
        {
            var entity = new SeatPrice
            {
                Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
                SeatType = dto.SeatType,
                TicketType = dto.TicketType,
                BasePrice = dto.BasePrice,
                Description = dto.Description
            };

            await _repository.AddAsync(entity);
        }

        public async Task UpdateAsync(SeatPriceDTOs dto)
        {
            // Lấy entity từ repository / DbContext
            var entity = await _repository.GetByIdAsync(dto.Id);
            if (entity == null)
                throw new KeyNotFoundException("SeatPrice not found");

            // Cập nhật từng trường
            entity.SeatType = dto.SeatType;
            entity.TicketType = dto.TicketType;
            entity.BasePrice = dto.BasePrice;
            entity.Description = dto.Description;

            // Lưu thay đổi qua repository
            await _repository.UpdateAsync(entity); // repository gọi SaveChangesAsync
        }

        public Task DeleteAsync(Guid id) => _repository.DeleteAsync(id);

    }
}