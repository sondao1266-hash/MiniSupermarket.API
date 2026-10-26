using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniSupermarket.API.Models
{
    /// <summary>
    /// Thực thể quản lý tài khoản nhân viên và phân quyền trong hệ thống siêu thị
    /// Ánh xạ tương ứng với bảng [Users] trong SQL Server
    /// </summary>
    [Table("Users")]
    public class User
    {
        // 1. Khóa chính: Tự động tăng IDENTITY(1,1)
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        // 2. Tên đăng nhập: Bắt buộc, tối đa 50 ký tự, không được trùng lặp
        [Required(ErrorMessage = "Tên tài khoản không được để trống!")]
        [StringLength(50, ErrorMessage = "Tên tài khoản không vượt quá 50 ký tự!")]
        public string Username { get; set; } = string.Empty;

        // 3. Chuỗi băm mật khẩu bảo mật (Password Hash)
        [Required(ErrorMessage = "Mật khẩu không được để trống!")]
        public string PasswordHash { get; set; } = string.Empty;

        // 4. Họ tên đầy đủ của nhân viên
        [Required(ErrorMessage = "Họ tên không được để trống!")]
        [StringLength(100, ErrorMessage = "Họ tên không vượt quá 100 ký tự!")]
        public string FullName { get; set; } = string.Empty;

        // 5. Vai trò trong hệ thống: Admin, Cashier (Thu ngân), Warehouse (Thủ kho)
        [Required(ErrorMessage = "Vai trò không được để trống!")]
        [StringLength(50)]
        public string Role { get; set; } = string.Empty;

        // 6. Trạng thái tài khoản: true = Đang hoạt động, false = Bị khóa
        public bool IsActive { get; set; } = true;
    }
}
