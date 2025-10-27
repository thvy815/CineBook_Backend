using System;

namespace AuthService.Domain.DTOs
{
	// Đăng ký tài khoản mới
	public record RegisterDto(
		string Email,
		string Username,
		string Password,
		string Fullname,
		string Gender,
		DateTime DateOfBirth,
		string PhoneNumber,
		string NationalId
	);

	// Đăng nhập
	public record LoginDto(
		string Email,
		string Password
	);

	// Đổi mật khẩu 
	public record ChangePasswordDto(
		string CurrentPassword,
		string NewPassword
	);

	// Đặt lại mật khẩu (quên mật khẩu)
	public record ResetPasswordDto(
		string Email,
		string Otp,
		string NewPassword
	);

	// Kết quả trả về sau đăng nhập/đăng ký/refresh token
	public record AuthResultDto(
		bool Success,
		string? AccessToken = null,
		string? RefreshToken = null,
		string? Message = null,
		string? Error = null
	);
}
