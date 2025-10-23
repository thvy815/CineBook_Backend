using MovieService.Domain.Entities;
using MovieService.Domain.Interfaces;
using MovieService.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieService.Application.Services
{
	public class MovieBusiness
	{
		private readonly IMovieRepository _repository;

		public MovieBusiness(IMovieRepository repository)
		{
			_repository = repository;
		}

		public Task<IEnumerable<Movie>> GetAllAsync() => _repository.GetAllAsync();
		public Task<Movie?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);
		public Task AddAsync(Movie movie) => _repository.AddAsync(movie);
        public async Task UpdateMovieAsync(Movie movie)
        {
            await _repository.UpdateAsync(movie);
        }

        public async Task DeleteMovieAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
