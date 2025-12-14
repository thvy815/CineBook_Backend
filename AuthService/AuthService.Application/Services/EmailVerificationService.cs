using AuthService.Application.Client;
using AuthService.Domain.Entities;
using AuthService.Domain.DTOs;
using AuthService.Domain.Interfaces;

namespace AuthService.Application.Services
{
    public class EmailVerificationService : IEmailVerificationService
    {
        private readonly IUserRepository _userRepo;
        private readonly IEmailVerificationTokenRepository _tokenRepo;
        private readonly IEmailService _emailService;
        private readonly IUserProfileClient _userProfileClient;

        public EmailVerificationService(
            IUserRepository userRepo,
            IEmailVerificationTokenRepository tokenRepo,
            IEmailService emailService,
            IUserProfileClient userProfileClient)
        {
            _userRepo = userRepo;
            _tokenRepo = tokenRepo;
            _emailService = emailService;
            _userProfileClient = userProfileClient;
        }

        public async Task<(bool Success, string Message)> VerifyEmailAsync(string token)
        {
            var tokenEntity = await _tokenRepo.GetByTokenAsync(token);

            if (tokenEntity == null || tokenEntity.Used)
                return (false, "Token không hợp lệ");

            if (tokenEntity.ExpiresAt < DateTime.UtcNow)
                return (false, "Token đã hết hạn");

            var user = tokenEntity.User;
            user.EmailVerified = true;
            user.Status = "ACTIVE";

            tokenEntity.Used = true;

            await _userRepo.UpdateAsync(user);
            await _tokenRepo.SaveChangesAsync();

            // TẠO PROFILE SAU KHI VERIFY
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

            return (true, "Xác thực email thành công");
        }

        public async Task<(bool Success, string Message)> ResendVerificationEmailAsync(string email)
        {
            var user = await _userRepo.GetByEmailAsync(email);
            if (user == null)
                return (false, "Email không tồn tại");

            if (user.EmailVerified)
                return (false, "Email đã được xác thực");

            var token = new EmailVerificationToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = Guid.NewGuid().ToString(),
                ExpiresAt = DateTime.UtcNow.AddMinutes(30)
            };

            await _tokenRepo.AddAsync(token);
            await _tokenRepo.SaveChangesAsync();

            await _emailService.SendVerifyEmailAsync(user.Email, token.Token);

            return (true, "Đã gửi lại email xác thực");
        }
    }
}
