using AuthService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Interfaces
{
	public interface IRoleRepository
	{
		Task<Role> GetByIdAsync(int id);
		Task<Role> GetByNameAsync(string name);
		Task<IEnumerable<Role>> GetAllAsync();
		Task CreateAsync(Role role);
	}
}
