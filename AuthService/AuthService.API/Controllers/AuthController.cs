using AuthService.Domain.DTOs;
using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace AuthService.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _auth;
		public AuthController(IAuthService auth) { _auth = auth; }

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterDto dto)
		{
			var res = await _auth.RegisterAsync(dto);
			if (!res.Success) return BadRequest(new { message = res.Message });
			return Ok(new { accessToken = res.AccessToken, refreshToken = res.RefreshToken });
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginDto dto)
		{
			var res = await _auth.LoginAsync(dto);
			if (!res.Success) return Unauthorized(new { message = res.Message });
			return Ok(new { accessToken = res.AccessToken, refreshToken = res.RefreshToken });
		}

		[HttpPost("refresh-token")]
		public async Task<IActionResult> Refresh([FromBody] string refreshToken)
		{
			var res = await _auth.RefreshTokenAsync(refreshToken);
			if (!res.Success) return Unauthorized(new { message = res.Message });
			return Ok(new { accessToken = res.AccessToken, refreshToken = res.RefreshToken });
		}

		[Authorize]
		[HttpPost("change-password")]
		public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
		{
			var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub));
			var ok = await _auth.ChangePasswordAsync(userId, dto);
			if (!ok) return BadRequest(new { message = "Mật khẩu hiện tại không đúng" });
			return Ok(new { message = "Đổi mật khẩu thành công" });
		}

		[HttpPost("forgot-password")]
		public async Task<IActionResult> ForgotPassword([FromBody] string email)
		{
			var ok = await _auth.SendPasswordResetOtpAsync(email);
			if (!ok) return BadRequest(new { message = "Email không tồn tại" });
			return Ok(new { message = "Đã gửi mã OTP tới email (nếu email tồn tại hệ thống)" });
		}

		[HttpPost("reset-password")]
		public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
		{
			var ok = await _auth.VerifyOtpAndResetPasswordAsync(dto);
			if (!ok) return BadRequest(new { message = "OTP không hợp lệ hoặc đã hết hạn" });
			return Ok(new { message = "Đặt lại mật khẩu thành công" });
		}

		[Authorize]
		[HttpDelete("account")]
		public async Task<IActionResult> DeleteAccount()
		{
			var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub));
			var ok = await _auth.DeleteAccountAsync(userId);
			if (!ok) return NotFound();
			return Ok(new { message = "Tài khoản đã được xóa" });
		}
	}
}
