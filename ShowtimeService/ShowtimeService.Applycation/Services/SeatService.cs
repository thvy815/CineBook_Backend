using ShowtimeService.Domain.Entities;
using ShowtimeService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShowtimeService.Application.Services
{
    public class SeatService
    {
        private readonly ISeatRepository _seatRepository;

        public SeatService(ISeatRepository seatRepository)
        {
            _seatRepository = seatRepository;
        }

        public async Task<IEnumerable<Seat>> GetAllAsync()
            => await _seatRepository.GetAllAsync();

        public async Task<Seat> GetByIdAsync(Guid id)
            => await _seatRepository.GetByIdAsync(id);

        public async Task<IEnumerable<Seat>> GetByRoomIdAsync(Guid roomId)
            => await _seatRepository.GetByRoomIdAsync(roomId);

        public async Task AddAsync(Seat seat)
            => await _seatRepository.AddAsync(seat);

        public async Task UpdateAsync(Seat seat)
            => await _seatRepository.UpdateAsync(seat);

        public async Task DeleteAsync(Guid id)
            => await _seatRepository.DeleteAsync(id);
    }
}
