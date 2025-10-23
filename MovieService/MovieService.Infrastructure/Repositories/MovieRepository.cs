using Microsoft.EntityFrameworkCore;
using MovieService.Domain.Entities;
using MovieService.Domain.Interfaces;
using MovieService.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieService.Infrastructure.Repositories
{
	public class MovieRepository : IMovieRepository
	{
		private readonly MovieDbContext _context;

		public MovieRepository(MovieDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<Movie>> GetAllAsync() => await _context.Movies.ToListAsync();

		public async Task<Movie?> GetByIdAsync(int id) => await _context.Movies.FindAsync(id);

		public async Task AddAsync(Movie movie)
		{
			_context.Movies.Add(movie);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateAsync(Movie movie)
		{
			_context.Movies.Update(movie);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(int id)
		{
			var movie = await _context.Movies.FindAsync(id);
			if (movie != null)
			{
				_context.Movies.Remove(movie);
				await _context.SaveChangesAsync();
			}
		}

	}
}
