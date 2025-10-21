using Microsoft.EntityFrameworkCore;
using MovieService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieService.Infrastructure.Data
{
	public class MovieDbContext : DbContext
	{
		public MovieDbContext(DbContextOptions<MovieDbContext> options) : base(options) { }

		public DbSet<Movie> Movies => Set<Movie>();
	}
}
