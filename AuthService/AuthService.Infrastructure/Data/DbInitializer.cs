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
					new Role { Name = "Customer", Description = "Default customer role" }
				);
				await context.SaveChangesAsync();
			}
		}
	}
}
