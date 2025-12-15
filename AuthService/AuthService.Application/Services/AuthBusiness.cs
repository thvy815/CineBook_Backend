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


		public AuthBusiness(IUserRepository userRepo, IRoleRepository roleRepo,
							 ITokenService tokenService, AuthDbContext db,
							 IEmailService emailService, IConfiguration config)
		{
			_userRepo = userRepo;
			_roleRepo = roleRepo;
			_tokenService = tokenService;
			_db = db;
			_emailService = emailService;
			_config = config;
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
                Status = "PENDING",
				EmailVerified = false
            };

            await _userRepo.CreateAsync(user);

            // Generate verify token
            var verifyToken = await _tokenService.GenerateEmailVerifyTokenAsync(user);

            await _emailService.SendVerifyEmailAsync(user.Email, verifyToken.Token);

            return new AuthResultDto(
                true,
                Message: "Đăng ký thành công, vui lòng kiểm tra email để xác thực"
            );
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

            if (user.Status != "ACTIVE")
                return new AuthResultDto(false, Message: "Tài khoản chưa được kích hoạt");

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
				return new AuthResultDto(false, Message: "Mật khẩu không đúng");

			var accessToken = _tokenService.GenerateAccessToken(user);
			var refresh = await _tokenService.GenerateRefreshTokenAsync(user, TimeSpan.FromDays(7));

            var userDto = new UserDto(user.Id, user.Username, user.Fullname, user.Email);

            return new AuthResultDto(true, accessToken, refresh.Token, User: userDto);
		}

        public async Task<PagedResponse<UserListResponse>> GetUsersAsync(
        string? keyword, string? status, string? role,
        int page, int size, string? sortBy, string? sortType)
        {
            // Default sorting
            var allowedSort = new List<string> { "CreatedAt", "Username", "Email", "Status" };
            var sortField = !string.IsNullOrWhiteSpace(sortBy) && allowedSort.Contains(sortBy)
                ? sortBy
                : "CreatedAt";

            var direction = (!string.IsNullOrWhiteSpace(sortType) && sortType.Equals("ASC", StringComparison.OrdinalIgnoreCase))
                ? true : false; // true = ascending, false = descending

            var query = _db.Users
                .Include(u => u.Role)
                .AsQueryable();

            // Keyword search
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim().ToLower();
                query = query.Where(u =>
                    u.Username.ToLower().Contains(kw) ||
                    u.Email.ToLower().Contains(kw) ||
                    (u.PhoneNumber != null && u.PhoneNumber.ToLower().Contains(kw))
                );
            }

            // Role filter
            if (!string.IsNullOrWhiteSpace(role))
            {
                var roleNormalized = role.Trim().ToLower();
                query = query.Where(u => u.Role != null && u.Role.Name.ToLower() == roleNormalized);
            }

            // Status filter
            if (!string.IsNullOrWhiteSpace(status))
            {
                var statusNormalized = status.Trim().ToLower();
                query = query.Where(u => u.Status.ToLower() == statusNormalized);
            }

            // Sorting
            query = sortField switch
            {
                "Username" => direction ? query.OrderBy(u => u.Username) : query.OrderByDescending(u => u.Username),
                "Email" => direction ? query.OrderBy(u => u.Email) : query.OrderByDescending(u => u.Email),
                "Status" => direction ? query.OrderBy(u => u.Status) : query.OrderByDescending(u => u.Status),
                _ => direction ? query.OrderBy(u => u.CreatedAt) : query.OrderByDescending(u => u.CreatedAt)
            };

            // Paging
            var totalElements = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalElements / size);
            var skip = (page - 1) * size;

            var data = await query.Skip(skip)
                                  .Take(size)
                                  .ToListAsync();

            return new PagedResponse<UserListResponse>
            {
                Data = data.Select(UserListResponse.FromEntity).ToList(),
                Page = page,
                Size = size,
                TotalElements = totalElements,
                TotalPages = totalPages
            };
        }

        public async Task<bool> UpdateUserRoleStatusAsync(
    Guid userId,
    UpdateUserRoleStatusDto dto)
        {
            var user = await _db.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return false;

            var role = await _db.Roles
                .FirstOrDefaultAsync(r => r.Name == dto.Role);

            if (role == null)
                return false;

            user.RoleId = role.Id;
            user.Status = dto.Status;
            user.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
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

        public async Task<bool> AdminCreateUserAsync(AdminCreateUserDto dto)
        {
            if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
                return false;

            if (await _db.Users.AnyAsync(u => u.Username == dto.Username))
                return false;

            var role = await _db.Roles.FirstOrDefaultAsync(r => r.Name == dto.Role);
            if (role == null)
                return false;

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                Username = dto.Username,
                Fullname = dto.Username,
                PhoneNumber = "",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                RoleId = role.Id,
                Status = dto.Status,
                EmailVerified = true,
                CreatedAt = DateTime.UtcNow
            };


            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return true;
        }


    }
}
