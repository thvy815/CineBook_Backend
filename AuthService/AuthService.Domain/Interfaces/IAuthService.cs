using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthService.Domain.DTOs;

namespace AuthService.Domain.Interfaces
{
	public interface IAuthService
	{
		Task<AuthResultDto> RegisterAsync(RegisterDto dto);
		Task<AuthResultDto> LoginAsync(LoginDto dto);
		Task<AuthResultDto> RefreshTokenAsync(string refreshToken);
		Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordDto dto);
		Task<bool> SendPasswordResetOtpAsync(string email);
		Task<bool> VerifyOtpAndResetPasswordAsync(ResetPasswordDto dto);
		Task<bool> DeleteAccountAsync(Guid userId);
        Task<PagedResponse<UserListResponse>> GetUsersAsync(
        string? keyword, string? status, string? role,
        int page, int size, string? sortBy, string? sortType);
        Task<bool> UpdateUserRoleStatusAsync(
    Guid userId,
    UpdateUserRoleStatusDto dto);

    }
}
