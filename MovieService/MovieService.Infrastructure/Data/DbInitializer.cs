using MovieService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieService.Infrastructure.Data
{
	public static class DbInitializer
	{
		public static void Initialize(MovieDbContext context)
		{
			context.Database.EnsureCreated();

			if (context.Movies.Any()) return; // Đã có dữ liệu rồi thì bỏ qua

			//var movies = new List<Movie>
			//{
			//	new Movie
			//	{
			//		Title = "Inception",
			//		Overview = "A skilled thief enters the dreams of others to steal secrets.",
			//		ReleaseDate = DateTime.SpecifyKind(new DateTime(2010, 7, 16), DateTimeKind.Utc)
			//	},
			//	new Movie
			//	{
			//		Title = "Interstellar",
			//		Overview = "A team travels through a wormhole to find a new home for humanity.",
			//		ReleaseDate = DateTime.SpecifyKind(new DateTime(2014, 11, 7), DateTimeKind.Utc)
			//	},
			//	new Movie
			//	{
			//		Title = "The Dark Knight",
			//		Overview = "Batman faces the Joker in a battle for Gotham’s soul.",
			//		ReleaseDate = DateTime.SpecifyKind(new DateTime(2008, 7, 18), DateTimeKind.Utc)
			//	},
			//	new Movie
			//	{
			//		Title = "Oppenheimer",
			//		Overview = "The story of J. Robert Oppenheimer and the creation of the atomic bomb.",
			//		ReleaseDate = DateTime.SpecifyKind(new DateTime(2023, 7, 21), DateTimeKind.Utc)
			//	}
		//	};

		//	context.Movies.AddRange(movies);
		//	context.SaveChanges();
		}
	}
}
