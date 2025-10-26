using BookingService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingService.Infrastructure.Data
{
    public class BookingDbContext : DbContext
    {
        public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options) { }

        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<BookingFnb> BookingFnbs => Set<BookingFnb>();
        public DbSet<BookingPromotion> BookingPromotions => Set<BookingPromotion>();
        public DbSet<BookingSeat> BookingSeats => Set<BookingSeat>();
        public DbSet<UsedPromotion> UsedPromotions => Set<UsedPromotion>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Booking
            modelBuilder.Entity<Booking>()
                .HasMany(b => b.BookingSeats)
                .WithOne(bs => bs.Booking!)
                .HasForeignKey(bs => bs.BookingId);

            modelBuilder.Entity<Booking>()
                .HasMany(b => b.BookingFnbs)
                .WithOne(bf => bf.Booking!)
                .HasForeignKey(bf => bf.BookingId);

            modelBuilder.Entity<Booking>()
                .HasMany(b => b.BookingPromotions)
                .WithOne(bp => bp.Booking!)
                .HasForeignKey(bp => bp.BookingId);

            modelBuilder.Entity<Booking>()
                .HasMany(b => b.UsedPromotions)
                .WithOne(up => up.Booking!)
                .HasForeignKey(up => up.BookingId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
