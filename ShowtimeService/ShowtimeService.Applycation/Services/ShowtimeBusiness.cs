using ShowtimeService.Domain.Entities;
using ShowtimeService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShowtimeService.Application.Services
{
    public class ShowtimeBusiness
    {
        private readonly IShowtimeRepository _showtimeRepository;

        public ShowtimeBusiness(IShowtimeRepository showtimeRepository)
        {
            _showtimeRepository = showtimeRepository;
        }

        public async Task<IEnumerable<Showtime>> GetAllAsync()
            => await _showtimeRepository.GetAllAsync();

        public async Task<Showtime> GetByIdAsync(Guid id)
            => await _showtimeRepository.GetByIdAsync(id);

        public async Task<IEnumerable<Showtime>> GetByTheaterIdAsync(Guid theaterId)
            => await _showtimeRepository.GetByTheaterIdAsync(theaterId);

        public async Task<IEnumerable<Showtime>> GetByMovieIdAsync(Guid movieId)
            => await _showtimeRepository.GetByMovieIdAsync(movieId);

        public async Task<IEnumerable<Showtime>> GetByDateAsync(DateTime date)
            => await _showtimeRepository.GetByDateAsync(date);

        public async Task AddAsync(Showtime showtime)
            => await _showtimeRepository.AddAsync(showtime);

        public async Task UpdateAsync(Showtime showtime)
            => await _showtimeRepository.UpdateAsync(showtime);

        public async Task DeleteAsync(Guid id)
            => await _showtimeRepository.DeleteAsync(id);
    }
}
