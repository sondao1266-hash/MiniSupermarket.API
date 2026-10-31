using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MiniSupermarket.API.Models
{
    /// <summary>
    /// Thực thể quản lý khách hàng thân thiết và điểm thưởng tích lũy tại siêu thị
    /// Ánh xạ tương ứng với bảng [Customers] trong SQL Server
    /// </summary>
    [Table("Customers")]
    public class Customer
    {
        // 1. Khóa chính tự tăng IDENTITY(1,1)
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustomerId { get; set; }

        // 2. Họ và tên khách hàng
        [Required(ErrorMessage = "Tên khách hàng không được để trống!")]
        [StringLength(100, ErrorMessage = "Tên khách hàng không vượt quá 100 ký tự!")]
        public string CustomerName { get; set; } = string.Empty;

        // 3. Số điện thoại định danh khách hàng (Duy nhất, dùng tra cứu tích điểm tại quầy)
        [Required(ErrorMessage = "Số điện thoại không được để trống!")]
        [StringLength(15, ErrorMessage = "Số điện thoại không vượt quá 15 ký tự!")]
        public string PhoneNumber { get; set; } = string.Empty;

        // 4. Địa chỉ khách hàng (Cho phép null)
        [StringLength(200)]
        public string? Address { get; set; }

        // 5. Điểm thưởng tích lũy (Mặc định là 0 khi đăng ký mới)
        public int RewardPoints { get; set; } = 0;

        // 6. Hạng thẻ thành viên: Chuẩn, Bạc, Vàng, Bạch Kim
        [StringLength(50)]
        public string MembershipRank { get; set; } = "Chuẩn";

        // Thuộc tính điều hướng: Một khách hàng có thể có nhiều hóa đơn mua hàng
        [JsonIgnore]
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
