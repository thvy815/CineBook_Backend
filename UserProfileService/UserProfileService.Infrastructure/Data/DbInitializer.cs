using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using UserProfileService.Domain.Entities;

namespace UserProfileService.Infrastructure.Data
{
	public static class DbInitializer
	{
		public static async Task SeedAsync(UserProfileDbContext context)
		{
			// Seed ranks 
			if (!context.UserRanks.Any())
			{
				var now = DateTime.UtcNow;
				context.UserRanks.AddRange(
					new UserRank { Id = Guid.NewGuid(), Name = "Bronze", MinPoints = 0, MaxPoints = 999, DiscountRate = 0m, CreatedAt = now },
					new UserRank { Id = Guid.NewGuid(), Name = "Silver", MinPoints = 1000, MaxPoints = 4999, DiscountRate = 5.00m, CreatedAt = now },
					new UserRank { Id = Guid.NewGuid(), Name = "Gold", MinPoints = 5000, MaxPoints = 19999, DiscountRate = 10.00m, CreatedAt = now }
				);
				await context.SaveChangesAsync();
			}

			// Seed user profiles 
			if (!context.UserProfiles.Any())
			{
				var bronze = context.UserRanks.OrderBy(r => r.MinPoints).FirstOrDefault();

				var profile = new UserProfile
				{
					Id = Guid.NewGuid(),
					UserId = Guid.NewGuid(), // sample AuthService id
					Email = "jane.doe@example.com",
					Username = "janedoe",
					Fullname = "Jane Doe",
					AvatarUrl = "default-avatar.png", 
					Gender = "female",
					DateOfBirth = new DateTime(1992, 5, 15),
					PhoneNumber = "0123456789",
					NationalId = "123456789",
					Address = "Hanoi, Vietnam",
					LoyaltyPoint = 120,
					RankId = bronze.Id,
					Status = "ACTIVE",
					CreatedAt = DateTime.UtcNow,
					UpdatedAt = DateTime.UtcNow
				};

				context.UserProfiles.Add(profile);
				await context.SaveChangesAsync();

				// Seed manager sample
				context.ManagerProfiles.Add(new ManagerProfile
				{
					Id = Guid.NewGuid(),
					UserProfileId = profile.Id,
					ManagedCinemaId = Guid.NewGuid(),
					HireDate = new DateTime(2020, 1, 1),
					CreatedAt = DateTime.UtcNow,
					UpdatedAt = DateTime.UtcNow
				});

				// Seed staff sample
				context.StaffProfiles.Add(new StaffProfile
				{
					Id = Guid.NewGuid(),
					UserProfileId = profile.Id,
					ManagedCinemaId = Guid.NewGuid(),
					HireDate = new DateTime(2021, 6, 1),
					CreatedAt = DateTime.UtcNow,
					UpdatedAt = DateTime.UtcNow
				});

				// Seed favorite movies sample
				context.UserFavoriteMovies.Add(new UserFavoriteMovie
				{
					Id = Guid.NewGuid(),
					UserProfileId = profile.Id,
					TmdbId = 500,
					AddedAt = DateTime.UtcNow
				});

				await context.SaveChangesAsync();
			}
		}
	}
}
