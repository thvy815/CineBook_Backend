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
        public MovieDbContext(DbContextOptions<MovieDbContext> options)
            : base(options) { }

        public DbSet<Movie> Movies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Movie>(entity =>
            {
                entity.ToTable("movie_details"); // hoặc "movie_summaries" tùy chọn
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.TmdbId).HasColumnName("tmdb_id");
                entity.Property(e => e.Title).HasColumnName("title");
                entity.Property(e => e.Age).HasColumnName("age");
                entity.Property(e => e.Status).HasColumnName("status");
                entity.Property(e => e.SpokenLanguages).HasColumnName("spoken_languages");
                entity.Property(e => e.Country).HasColumnName("country");
                entity.Property(e => e.Time).HasColumnName("time");
                entity.Property(e => e.Genres).HasColumnName("genres");
                entity.Property(e => e.Crew).HasColumnName("crew");
                entity.Property(e => e.Cast).HasColumnName("cast");
                entity.Property(e => e.ReleaseDate).HasColumnName("release_date");
                entity.Property(e => e.Overview).HasColumnName("overview");
                entity.Property(e => e.PosterUrl).HasColumnName("poster_url");
                entity.Property(e => e.Trailer).HasColumnName("trailer");
            });
        }
    }
}
