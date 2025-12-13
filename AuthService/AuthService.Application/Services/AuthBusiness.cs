using AuthService.Domain.DTOs;
using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AuthService.Application.Client;

namespace AuthService.Application.Services
{
	public class AuthBusiness : IAuthService
	{
		private readonly IUserRepository _userRepo;
		private readonly IRoleRepository _roleRepo;
		private readonly ITokenService _tokenService;
		private readonly AuthDbContext _db;
		private readonly IEmailService _emailService;
		private readonly IConfiguration _config;
		private readonly IUserProfileClient _userProfileClient;

		public AuthBusiness(IUserRepository userRepo, IRoleRepository roleRepo,
							 ITokenService tokenService, AuthDbContext db,
							 IEmailService emailService, IConfiguration config, 
							 IUserProfileClient userProfileClient)
		{
			_userRepo = userRepo;
			_roleRepo = roleRepo;
			_tokenService = tokenService;
			_db = db;
			_emailService = emailService;
			_config = config;
			_userProfileClient = userProfileClient;
		}

        public async Task<AuthResultDto> RegisterAsync(RegisterDto dto)
        {
            if (await _userRepo.ExistsByEmailAsync(dto.Email))
                return new AuthResultDto(false, Message: "Email đã được sử dụng");

            if (await _userRepo.ExistsByPhone(dto.PhoneNumber))
                return new AuthResultDto(false, Message: "SĐT đã được sử dụng");

            if (await _userRepo.ExistsByUsername(dto.Username))
                return new AuthResultDto(false, Message: "Username đã tồn tại");

            var customerRole = await _roleRepo.GetByNameAsync("Customer")
                ?? await _roleRepo.GetByNameAsync("customer");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                Username = dto.Username,
                Fullname = dto.Fullname,
                PhoneNumber = dto.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                RoleId = customerRole?.Id ?? 2,
                CreatedAt = DateTime.UtcNow,
                Status = "ACTIVE"
            };

            await _userRepo.CreateAsync(user);

            try
            {
                // generate tokens
                var accessToken = _tokenService.GenerateAccessToken(user);
                var refresh = await _tokenService.GenerateRefreshTokenAsync(user, TimeSpan.FromDays(7));

                var profileDto = new UserProfileCreateDTO
                {
                    Email = user.Email,
                    Username = user.Username,
                    Fullname = user.Fullname,
                    PhoneNumber = user.PhoneNumber,
                    LoyaltyPoint = 0,
                    Status = "ACTIVE"
                };

                await _userProfileClient.CreateUserProfileAsync(user.Id, profileDto);

                return new AuthResultDto(true, accessToken, refresh.Token);
            }
            catch (Exception ex)
            {
                // rollback user nếu profile fail
                await _userRepo.DeleteAsync(user);

                // log để debug
                Console.WriteLine($"Create profile failed: {ex.Message}");
                Console.WriteLine("==== USER PROFILE SERVICE ERROR ====");
                Console.WriteLine($"UserId: {user.Id}");
                Console.WriteLine($"Email: {user.Email}");
                Console.WriteLine(ex.Message);
                Console.WriteLine("===================================");

                return new AuthResultDto(
                    false,
                    Message: "Đăng ký thất bại, vui lòng thử lại sau"
                );
            }
        }

        public async Task<AuthResultDto> LoginAsync(LoginDto dto)
		{
            if (string.IsNullOrWhiteSpace(dto.Identifier) || string.IsNullOrWhiteSpace(dto.Password))
            {
                return new AuthResultDto(false, Message: "Thiếu thông tin đăng nhập");
            }

            User? user = null;

            switch (LoginIdentifierHelper.Detect(dto.Identifier))
            {
                case LoginIdentifierType.Email:
                    user = await _userRepo.GetByEmailAsync(dto.Identifier);
                    break;

                case LoginIdentifierType.Phone:
                    user = await _userRepo.GetByPhoneAsync(dto.Identifier);
                    break;

                case LoginIdentifierType.Username:
                    user = await _userRepo.GetByUsernameAsync(dto.Identifier);
                    break;
            }

			if (user == null) return new AuthResultDto(false, Message: "Tài khoản không tồn tại");

			if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
				return new AuthResultDto(false, Message: "Mật khẩu không đúng");

			var accessToken = _tokenService.GenerateAccessToken(user);
			var refresh = await _tokenService.GenerateRefreshTokenAsync(user, TimeSpan.FromDays(7));

            var userDto = new UserDto(user.Id, user.Username, user.Fullname, user.Email);

            return new AuthResultDto(true, accessToken, refresh.Token, User: userDto);
		}

		public async Task<AuthResultDto> RefreshTokenAsync(string refreshToken)
		{
			var existing = await _db.RefreshTokens.Include(r => r.User).FirstOrDefaultAsync(r => r.Token == refreshToken);
			if (existing == null || existing.ExpiresAt < DateTime.UtcNow)
				return new AuthResultDto(false, Message: "Refresh token không hợp lệ");

			var user = existing.User;
			var accessToken = _tokenService.GenerateAccessToken(user);

			// optionally rotate refresh token
			_db.RefreshTokens.Remove(existing);
			var newRt = new RefreshToken
			{
				Id = Guid.NewGuid(),
				UserId = user.Id,
				Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
				ExpiresAt = DateTime.UtcNow.AddDays(7),
				CreatedAt = DateTime.UtcNow
			};
			_db.RefreshTokens.Add(newRt);
			await _db.SaveChangesAsync();

			return new AuthResultDto(true, accessToken, newRt.Token);
		}

		public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordDto dto)
		{
			var user = await _userRepo.GetByIdAsync(userId);
			if (user == null) return false;

			if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
				return false;

			user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
			user.UpdatedAt = DateTime.UtcNow;
			await _userRepo.UpdateAsync(user);
			return true;
		}

		public async Task<bool> SendPasswordResetOtpAsync(string email)
		{
			var user = await _userRepo.GetByEmailAsync(email);
			if (user == null) return false;

			var otp = new Random().Next(100000, 999999).ToString();
			var pr = new PasswordResetOtp
			{
				Id = Guid.NewGuid(),
				Email = email,
				Otp = otp,
				ExpiresAt = DateTime.UtcNow.AddMinutes(10),
				CreatedAt = DateTime.UtcNow
			};
			_db.PasswordResetOtps.Add(pr);
			await _db.SaveChangesAsync();

			await _emailService.SendEmailAsync(email, "Mã OTP đặt lại mật khẩu", $"Mã OTP của bạn là: {otp}. Hết hạn sau 10 phút.");

			return true;
		}

		public async Task<bool> VerifyOtpAndResetPasswordAsync(ResetPasswordDto dto)
		{
			var record = await _db.PasswordResetOtps
								  .Where(r => r.Email == dto.Email && r.Otp == dto.Otp)
								  .OrderByDescending(r => r.CreatedAt)
								  .FirstOrDefaultAsync();

			if (record == null || record.ExpiresAt < DateTime.UtcNow) return false;

			var user = await _userRepo.GetByEmailAsync(dto.Email);
			if (user == null) return false;

			user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
			user.UpdatedAt = DateTime.UtcNow;
			await _userRepo.UpdateAsync(user);

			// optionally remove used OTPs
			_db.PasswordResetOtps.RemoveRange(_db.PasswordResetOtps.Where(r => r.Email == dto.Email));
			await _db.SaveChangesAsync();

			return true;
		}

		public async Task<bool> DeleteAccountAsync(Guid userId)
		{
			var user = await _userRepo.GetByIdAsync(userId);
			if (user == null) return false;
			await _userRepo.DeleteAsync(user);
			return true;
		}
	}
}
