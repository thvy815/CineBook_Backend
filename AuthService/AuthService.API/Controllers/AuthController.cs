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
        private readonly IEmailVerificationService _service;

        public AuthController(IAuthService auth, IEmailVerificationService service) 
		{ 
			_auth = auth; 
			_service = service; 
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterDto dto)
		{
			var res = await _auth.RegisterAsync(dto);
			if (!res.Success) return BadRequest(new { message = res.Message });
			return Ok(new { message = "Đăng ký thành công. Vui lòng kiểm tra email để xác thực tài khoản." });
		}

        // Verify email
        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            var res = await _service.VerifyEmailAsync(token);
            if (!res.Success)
                return BadRequest(new { message = res.Message });

            return Ok(new { message = res.Message });
        }

        // Resend verify email
        [HttpPost("resend-verification")]
        public async Task<IActionResult> Resend([FromBody] ResendVerificationEmailDto dto)
        {
            var res = await _service.ResendVerificationEmailAsync(dto.Email);
            if (!res.Success)
                return BadRequest(new { message = res.Message });

            return Ok(new { message = res.Message });
        }

        [HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginDto dto)
		{
			var res = await _auth.LoginAsync(dto);
			if (!res.Success) return Unauthorized(new { message = res.Message });
			return Ok(new { accessToken = res.AccessToken, refreshToken = res.RefreshToken, user = res.User});
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

        [HttpGet("users")]
        public async Task<ActionResult<PagedResponse<UserListResponse>>> GetAllUsers(
        [FromQuery] string? keyword,
        [FromQuery] string? status,
        [FromQuery] string? role,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortType = null)
        {
            // TODO: Replace with your own admin check
            //if (!User.IsInRole("Admin"))
            //    return Forbid();

            var users = await _auth.GetUsersAsync(keyword, status, role, page, size, sortBy, sortType);
            return Ok(users);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("users/{userId}")]
        public async Task<IActionResult> UpdateUserRoleStatus(
    Guid userId,
    [FromBody] UpdateUserRoleStatusDto dto)
        {
            var ok = await _auth.UpdateUserRoleStatusAsync(userId, dto);

            if (!ok)
                return BadRequest(new { message = "Cập nhật role hoặc status thất bại" });

            return Ok(new { message = "Cập nhật role và status thành công" });
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

        [Authorize(Roles = "Admin")]
        [HttpPost("users")]
        public async Task<IActionResult> AdminCreateUser(
    [FromBody] AdminCreateUserDto dto)
        {
            var ok = await _auth.AdminCreateUserAsync(dto);

            if (!ok)
                return BadRequest(new { message = "Tạo tài khoản thất bại (email, username hoặc role không hợp lệ)" });

            return Ok(new { message = "Tạo tài khoản thành công" });
        }


    }
}
