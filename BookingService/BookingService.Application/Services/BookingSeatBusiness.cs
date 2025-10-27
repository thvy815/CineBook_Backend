using BookingService.Domain.DTOs;
using BookingService.Domain.Entities;
using BookingService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingService.Application.Services
{
    public class BookingSeatBusiness
    {
        private readonly IBookingSeatRepository _repository;

        public BookingSeatBusiness(IBookingSeatRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<BookingSeatDTOs>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => new BookingSeatDTOs
            {
                Id = e.Id,
                BookingId = e.BookingId,
                SeatId = e.SeatId,
                SeatType = e.SeatType,
                TicketType = e.TicketType,
                Price = e.Price,
                CreatedAt = e.CreatedAt
            });
        }

        public async Task<BookingSeatDTOs?> GetByIdAsync(Guid id)
        {
            var e = await _repository.GetByIdAsync(id);
            if (e == null) return null;

            return new BookingSeatDTOs
            {
                Id = e.Id,
                BookingId = e.BookingId,
                SeatId = e.SeatId,
                SeatType = e.SeatType,
                TicketType = e.TicketType,
                Price = e.Price,
                CreatedAt = e.CreatedAt
            };
        }

        public async Task AddAsync(BookingSeatDTOs dto)
        {
            var entity = new BookingSeat
            {
                Id = dto.Id,
                BookingId = dto.BookingId,
                SeatId = dto.SeatId,
                SeatType = dto.SeatType,
                TicketType = dto.TicketType,
                Price = dto.Price,
                CreatedAt = dto.CreatedAt
            };
            await _repository.AddAsync(entity);
        }

        public async Task UpdateAsync(BookingSeatDTOs dto)
        {
            var entity = new BookingSeat
            {
                Id = dto.Id,
                BookingId = dto.BookingId,
                SeatId = dto.SeatId,
                SeatType = dto.SeatType,
                TicketType = dto.TicketType,
                Price = dto.Price,
                CreatedAt = dto.CreatedAt
            };

            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
