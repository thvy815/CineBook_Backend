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
	public class UserRepository : IUserRepository
	{
		private readonly AuthDbContext _db;
		public UserRepository(AuthDbContext db) { _db = db; }

		public async Task CreateAsync(User user)
		{
			_db.Users.Add(user);
			await _db.SaveChangesAsync();
		}

		public async Task DeleteAsync(User user)
		{
			_db.Users.Remove(user);
			await _db.SaveChangesAsync();
		}

		public async Task<User> GetByEmailAsync(string email)
		{
			return await _db.Users.Include(u => u.Role)
								  .Include(u => u.RefreshTokens)
								  .FirstOrDefaultAsync(u => u.Email == email);
		}

        public async Task<User> GetByPhoneAsync(string phone)
        {
            return await _db.Users.Include(u => u.Role)
                                  .Include(u => u.RefreshTokens)
                                  .FirstOrDefaultAsync(u => u.PhoneNumber == phone);
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return await _db.Users.Include(u => u.Role)
                                  .Include(u => u.RefreshTokens)
                                  .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User> GetByIdAsync(Guid id)
		{
			return await _db.Users.Include(u => u.Role)
								  .Include(u => u.RefreshTokens)
								  .FirstOrDefaultAsync(u => u.Id == id);
		}

		public async Task UpdateAsync(User user)
		{
			_db.Users.Update(user);
			await _db.SaveChangesAsync();
		}

		public async Task<bool> ExistsByEmailAsync(string email)
		{
			return await _db.Users.AnyAsync(u => u.Email == email);
		}

        public async Task<bool> ExistsByPhone(string phone)
        {
            return await _db.Users.AnyAsync(u => u.PhoneNumber == phone);
        }

        public async Task<bool> ExistsByUsername(string username)
        {
            return await _db.Users.AnyAsync(u => u.Username == username);
        }
    }
}
