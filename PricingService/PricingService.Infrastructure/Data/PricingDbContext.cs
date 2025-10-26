using Microsoft.EntityFrameworkCore;
using PricingService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricingService.Infrastructure.Data
{
    public class PricingDbContext : DbContext
    {
        public PricingDbContext(DbContextOptions<PricingDbContext> options) : base(options) { }

        public DbSet<SeatPrice> SeatPrices => Set<SeatPrice>();
        public DbSet<FnbItem> FnbItems => Set<FnbItem>();
        public DbSet<Promotion> Promotions => Set<Promotion>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // seat_price
            // Table: seat_price
            modelBuilder.Entity<SeatPrice>(entity =>
            {
                entity.ToTable("seat_price");

                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.SeatType)
                    .HasColumnName("seat_type")
                    .HasMaxLength(50);

                entity.Property(e => e.TicketType)
                    .HasColumnName("ticket_type")
                    .HasMaxLength(50);

                entity.Property(e => e.BasePrice)
                    .HasColumnName("base_price")
                    .HasColumnType("numeric(10,2)");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(255);
            });

            // fnb_item
            modelBuilder.Entity<FnbItem>(entity =>
            {
                entity.ToTable("fnb_item");
                entity.HasKey(e => e.id);

                entity.Property(e => e.id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.UnitPrice)
                      .HasColumnName("unit_price")
                      .HasColumnType("numeric(10,2)");
            });

            // promotion
            modelBuilder.Entity<Promotion>(entity =>
            {
                entity.ToTable("promotion");

                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Code).HasColumnName("code").HasMaxLength(50);
                entity.Property(e => e.DiscountType).HasColumnName("discount_type").HasMaxLength(20);
                entity.Property(e => e.DiscountValue).HasColumnName("discount_value").HasColumnType("numeric(10,2)");
                entity.Property(e => e.StartDate).HasColumnName("start_date");
                entity.Property(e => e.EndDate).HasColumnName("end_date");
                entity.Property(e => e.IsActive).HasColumnName("is_active");
                entity.Property(e => e.IsOneTimeUse).HasColumnName("is_one_time_use");
                entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(500);
            });
        }
    }
}
