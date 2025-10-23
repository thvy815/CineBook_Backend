using AuthService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Interfaces
{
	public interface IUserRepository
	{
		Task<User> GetByEmailAsync(string email);
		Task<User> GetByIdAsync(Guid id);
		Task CreateAsync(User user);
		Task UpdateAsync(User user);
		Task DeleteAsync(User user);
		Task<bool> ExistsByEmailAsync(string email);
	}
}
