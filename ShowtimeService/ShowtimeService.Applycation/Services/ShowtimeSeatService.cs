using ShowtimeService.Domain.Entities;
using ShowtimeService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShowtimeService.Application.Services
{
    public class ShowtimeSeatService
    {
        private readonly IShowtimeSeatRepository _showtimeSeatRepository;

        public ShowtimeSeatService(IShowtimeSeatRepository showtimeSeatRepository)
        {
            _showtimeSeatRepository = showtimeSeatRepository;
        }

        public async Task<IEnumerable<ShowtimeSeat>> GetAllAsync()
            => await _showtimeSeatRepository.GetAllAsync();

        public async Task<IEnumerable<ShowtimeSeat>> GetByShowtimeIdAsync(Guid showtimeId)
            => await _showtimeSeatRepository.GetByShowtimeIdAsync(showtimeId);

        public async Task<IEnumerable<ShowtimeSeat>> GetAvailableSeatsAsync(Guid showtimeId)
            => await _showtimeSeatRepository.GetAvailableSeatsAsync(showtimeId);

        public async Task UpdateSeatStatusAsync(Guid seatId, string status)
            => await _showtimeSeatRepository.UpdateSeatStatusAsync(seatId, status);
    }
}
