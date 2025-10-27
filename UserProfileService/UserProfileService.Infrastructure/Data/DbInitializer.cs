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
		}
	}
}
