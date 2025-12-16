using System;

namespace AuthService.Domain.DTOs
{
	// Đăng ký tài khoản mới
	public record RegisterDto(
		string Email,
		string Username,
		string Password,
		string Fullname,
		string PhoneNumber
	);

	// Đăng nhập
	public record LoginDto(
        string Identifier, // username | email | phone
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
		string? Error = null,
        UserDto? User = null 
    );

    public record UserDto(
		Guid Id,
		string Username,
		string Fullname,
		string Email, 
		int role
	);

    public class UpdateUserRoleStatusDto
    {
        public string Role { get; set; } = null!;
        public string Status { get; set; } = null!;
    }

    public class AdminCreateUserDto
    {
        public string Email { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Role { get; set; } = "Customer";
        public string Status { get; set; } = "ACTIVE";
    }

}
