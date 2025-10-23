using Microsoft.EntityFrameworkCore;
using MovieService.Domain.Entities;

namespace MovieService.Infrastructure.Data
{
    public class MovieDbContext : DbContext
    {
        public MovieDbContext(DbContextOptions<MovieDbContext> options) : base(options) { }

        public DbSet<MovieSummary> MovieSummaries { get; set; }
        public DbSet<MovieDetail> MovieDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MovieSummary>(entity =>
            {
                entity.ToTable("movie_summaries");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();  // auto-increment
            });

            modelBuilder.Entity<MovieDetail>(entity =>
            {
                entity.ToTable("movie_details");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
        }
    }
}
