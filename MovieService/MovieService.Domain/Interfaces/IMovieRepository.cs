using MovieService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieService.Domain.Interfaces
{
	public interface IMovieRepository
	{
		Task<IEnumerable<Movie>> GetAllAsync();
		Task<Movie?> GetByIdAsync(int id);
		Task AddAsync(Movie movie);
		Task UpdateAsync(Movie movie);
		Task DeleteAsync(int id);
	}
}
