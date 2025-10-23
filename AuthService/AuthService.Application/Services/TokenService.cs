using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Services
{
	public class TokenService : ITokenService
	{
		private readonly IConfiguration _config;
		private readonly AuthDbContext _db;

		public TokenService(IConfiguration config, AuthDbContext db)
		{
			_config = config;
			_db = db;
		}

		public string GenerateAccessToken(User user)
		{
			// load from .env
			var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
			var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");
			var secret = Environment.GetEnvironmentVariable("JWT_SECRET");
			var accessTokenExpMinutes = double.Parse(Environment.GetEnvironmentVariable("JWT_ACCESS_TOKEN_EXP_MINUTES") ?? "60");

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var claims = new List<Claim>
			{
				new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
				new Claim(ClaimTypes.Email, user.Email),
				new Claim(ClaimTypes.Name, user.Username ?? user.Email),
				new Claim(ClaimTypes.Role, user.Role?.Name ?? "Customer")
			};

			var token = new JwtSecurityToken(
				issuer: issuer,
				audience: audience,
				claims: claims,
				expires: DateTime.UtcNow.AddMinutes(accessTokenExpMinutes),
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		public async Task<RefreshToken> GenerateRefreshTokenAsync(User user, TimeSpan validFor)
		{
			// load from .env
			var refreshDays = int.Parse(Environment.GetEnvironmentVariable("JWT_REFRESH_TOKEN_EXP_DAYS") ?? "30");
			var duration = validFor == TimeSpan.Zero ? TimeSpan.FromDays(refreshDays) : validFor;

			var rt = new RefreshToken
			{
				Id = Guid.NewGuid(),
				UserId = user.Id,
				Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
				ExpiresAt = DateTime.UtcNow.Add(duration),
				CreatedAt = DateTime.UtcNow
			};

			_db.RefreshTokens.Add(rt);
			await _db.SaveChangesAsync();
			return rt;
		}
	}
}
