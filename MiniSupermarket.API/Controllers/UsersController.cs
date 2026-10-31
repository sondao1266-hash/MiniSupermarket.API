using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniSupermarket.API.Data;
using MiniSupermarket.API.Models;

namespace MiniSupermarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Chỉ Admin được quản trị tài khoản
    public class UsersController : ControllerBase
    {
        private const string DefaultPassword = "123456";
        private static readonly string[] Roles = { "Admin", "Cashier", "Warehouse" };

        private readonly SupermarketDbContext _db;

        public UsersController(SupermarketDbContext db)
        {
            _db = db;
        }

        // GET api/users
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _db.Users
                .AsNoTracking()
                .OrderBy(u => u.UserId)
                .Select(u => new
                {
                    id = u.UserId,
                    username = u.Username,
                    fullName = u.FullName,
                    role = u.Role,
                    isActive = u.IsActive
                })
                .ToListAsync();
            return Ok(list);
        }

        // POST api/users
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username)) return BadRequest(new { message = "Tên đăng nhập không được trống!" });
            if (string.IsNullOrWhiteSpace(dto.FullName)) return BadRequest(new { message = "Họ tên không được trống!" });
            if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 6)
                return BadRequest(new { message = "Mật khẩu tối thiểu 6 ký tự!" });
            if (!Roles.Contains(dto.Role))
                return BadRequest(new { message = "Vai trò không hợp lệ!" });

            string username = dto.Username.Trim();
            if (await _db.Users.AnyAsync(u => u.Username == username))
                return BadRequest(new { message = "Tên đăng nhập đã tồn tại!" });

            _db.Users.Add(new User
            {
                Username = username,
                FullName = dto.FullName.Trim(),
                PasswordHash = dto.Password,   // nếu đã dùng BCrypt: BCrypt.Net.BCrypt.HashPassword(dto.Password)
                Role = dto.Role,
                IsActive = true
            });
            await _db.SaveChangesAsync();

            return Ok(new { message = "Tạo tài khoản thành công!" });
        }

        // PUT api/users/5/reset-password
        [HttpPut("{id:int}/reset-password")]
        public async Task<IActionResult> ResetPassword(int id, [FromBody] ResetPasswordDto dto)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound(new { message = "Không tìm thấy tài khoản!" });

            string newPassword = string.IsNullOrWhiteSpace(dto.NewPassword) ? DefaultPassword : dto.NewPassword;
            if (newPassword.Length < 6) return BadRequest(new { message = "Mật khẩu tối thiểu 6 ký tự!" });

            user.PasswordHash = newPassword;   // nếu dùng BCrypt: BCrypt.Net.BCrypt.HashPassword(newPassword)
            await _db.SaveChangesAsync();
            return Ok(new { message = "Đã đặt lại mật khẩu!" });
        }

        // PUT api/users/5/toggle-lock
        [HttpPut("{id:int}/toggle-lock")]
        public async Task<IActionResult> ToggleLock(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound(new { message = "Không tìm thấy tài khoản!" });

            // Không cho Admin tự khóa chính mình
            if (string.Equals(user.Username, User.Identity?.Name, StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { message = "Bạn không thể khóa tài khoản đang đăng nhập!" });

            user.IsActive = !user.IsActive;
            await _db.SaveChangesAsync();
            return Ok(new { isActive = user.IsActive });
        }
    }

    public class CreateUserDto
    {
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    public class ResetPasswordDto
    {
        public string? NewPassword { get; set; }
    }
}