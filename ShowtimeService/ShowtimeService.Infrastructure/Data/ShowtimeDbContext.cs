using Microsoft.EntityFrameworkCore;
using ShowtimeService.Domain.Entities;

namespace ShowtimeService.Infrastructure.Data
{
    public class ShowtimeDbContext : DbContext
    {
        public ShowtimeDbContext(DbContextOptions<ShowtimeDbContext> options) : base(options) { }

        public DbSet<Province> Provinces { get; set; }
        public DbSet<Theater> Theaters { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Showtime> Showtimes { get; set; }
        public DbSet<ShowtimeSeat> ShowtimeSeats { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Province>()
                .HasMany(p => p.Theaters)
                .WithOne(t => t.Province)
                .HasForeignKey(t => t.ProvinceId);

            modelBuilder.Entity<Theater>()
                .HasMany(t => t.Rooms)
                .WithOne(r => r.Theater)
                .HasForeignKey(r => r.TheaterId);

            modelBuilder.Entity<Room>()
                .HasMany(r => r.Seats)
                .WithOne(s => s.Room)
                .HasForeignKey(s => s.RoomId);

            modelBuilder.Entity<Showtime>()
                .HasMany(s => s.ShowtimeSeats)
                .WithOne(ss => ss.Showtime)
                .HasForeignKey(ss => ss.ShowtimeId);
        }
    }
}
