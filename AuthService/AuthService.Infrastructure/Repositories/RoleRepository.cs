using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Repositories
{
	public class RoleRepository : IRoleRepository
	{
		private readonly AuthDbContext _db;
		public RoleRepository(AuthDbContext db) { _db = db; }

		public async Task CreateAsync(Role role)
		{
			_db.Roles.Add(role);
			await _db.SaveChangesAsync();
		}

		public async Task<IEnumerable<Role>> GetAllAsync()
		{
			return await _db.Roles.ToListAsync();
		}

		public async Task<Role> GetByIdAsync(int id)
		{
			return await _db.Roles.FindAsync(id);
		}

		public async Task<Role> GetByNameAsync(string name)
		{
			return await _db.Roles.FirstOrDefaultAsync(r => r.Name == name);
		}
	}
}
