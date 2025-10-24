using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthService.Domain.DTOs;

namespace AuthService.Domain.Interfaces
{
	public interface IUserProfileClient
	{
		Task CreateUserProfileAsync(Guid userId, UserProfileCreateDTO dto);
	}
}
