using AuthService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Interfaces
{
	public interface ITokenService
	{
		string GenerateAccessToken(User user);
		Task<RefreshToken> GenerateRefreshTokenAsync(User user, TimeSpan validFor);
        Task<EmailVerificationToken> GenerateEmailVerifyTokenAsync(User user);
    }
}
