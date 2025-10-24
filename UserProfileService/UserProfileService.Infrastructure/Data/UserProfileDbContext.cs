using Microsoft.EntityFrameworkCore;
using UserProfileService.Domain.Entities;

namespace UserProfileService.Infrastructure.Data
{
	public class UserProfileDbContext : DbContext
	{
		public UserProfileDbContext(DbContextOptions<UserProfileDbContext> options) : base(options) { }

		public DbSet<UserProfile> UserProfiles { get; set; }
		public DbSet<UserFavoriteMovie> UserFavoriteMovies { get; set; }
		public DbSet<UserRank> UserRanks { get; set; }
		public DbSet<ManagerProfile> ManagerProfiles { get; set; }
		public DbSet<StaffProfile> StaffProfiles { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// user_profiles
			modelBuilder.Entity<UserProfile>(b =>
			{
				b.HasOne(e => e.Rank)
				 .WithMany()
				 .HasForeignKey(e => e.RankId)
				 .OnDelete(DeleteBehavior.SetNull);
			});

			// user_favorite_movies
			modelBuilder.Entity<UserFavoriteMovie>(b =>
			{
				b.HasOne(e => e.UserProfile)
				 .WithMany(p => p.FavoriteMovies)
				 .HasForeignKey(e => e.UserProfileId)
				 .OnDelete(DeleteBehavior.Cascade);
			});

			// manager_profiles
			modelBuilder.Entity<ManagerProfile>(b =>
			{
				b.HasOne(e => e.UserProfile)
				 .WithMany()
				 .HasForeignKey(e => e.UserProfileId)
				 .OnDelete(DeleteBehavior.Cascade);
			});

			// staff_profiles
			modelBuilder.Entity<StaffProfile>(b =>
			{
				b.HasOne(e => e.UserProfile)
				 .WithMany()
				 .HasForeignKey(e => e.UserProfileId)
				 .OnDelete(DeleteBehavior.Cascade);
			});
		}
	}
}
