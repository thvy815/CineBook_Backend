using AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Data
{
	public static class DbInitializer
	{
		public static async Task InitializeAsync(AuthDbContext context)
		{
			await context.Database.MigrateAsync();

			if (!context.Roles.Any())
			{
				context.Roles.AddRange(
					new Role { Name = "Admin", Description = "Administrator" },
					new Role { Name = "Customer", Description = "Default customer role" },
					new Role { Name = "Guest", Description = "Buy tickets without registration" },
					new Role { Name = "Manager", Description = "Manager theater" },
					new Role { Name = "Staff", Description = "Staff theater" }
				);
				await context.SaveChangesAsync();
			}
		}
	}
}
