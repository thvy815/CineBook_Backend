using AuthService.Domain.Entities;
using AuthService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Repositories
{
	public class RefreshTokenRepository
	{
		private readonly AuthDbContext _db;
		public RefreshTokenRepository(AuthDbContext db) { _db = db; }

		public async Task AddAsync(RefreshToken token)
		{
			_db.RefreshTokens.Add(token);
			await _db.SaveChangesAsync();
		}

		public async Task<RefreshToken> GetByTokenAsync(string token)
		{
			return await _db.RefreshTokens.Include(t => t.User).FirstOrDefaultAsync(t => t.Token == token);
		}

		public async Task DeleteAsync(RefreshToken token)
		{
			_db.RefreshTokens.Remove(token);
			await _db.SaveChangesAsync();
		}
	}
}
