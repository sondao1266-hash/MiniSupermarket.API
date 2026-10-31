using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniSupermarket.API.Models
{
    /// <summary>
    /// Thực thể quản lý thông tin hóa đơn thanh toán tại quầy thu ngân
    /// Ánh xạ tương ứng với bảng [Orders] trong SQL Server
    /// </summary>
    [Table("Orders")]
    public class Order
    {
        // 1. Khóa chính tự tăng
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }

        // 2. Mã hóa đơn nghiệp vụ in trên hóa đơn (VD: HD20261001-001)
        [Required]
        [StringLength(50)]
        public string OrderCode { get; set; } = string.Empty;

        // 3. Ngày giờ lập hóa đơn
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // 4. Khóa ngoại tham chiếu về khách hàng (Khách lẻ hoặc Khách thành viên)
        [Required]
        public int CustomerId { get; set; }

        // 5. Tên tài khoản thu ngân lập hóa đơn
        [Required]
        [StringLength(50)]
        public string CashierUsername { get; set; } = string.Empty;

        // 6. Tổng số lượng món hàng trong hóa đơn
        public int TotalItems { get; set; }

        // 7. Tổng tiền thanh toán thực tế của hóa đơn
        [Column(TypeName = "decimal(18,2)")]
        public decimal FinalAmount { get; set; }

        // 8. Số điểm thưởng cộng cho khách từ hóa đơn này
        public int RewardPoints { get; set; } = 0;

        // Thuộc tính điều hướng về khách hàng
        [ForeignKey(nameof(CustomerId))]
        public virtual Customer? Customer { get; set; }

        // Danh sách các dòng mặt hàng chi tiết trong hóa đơn
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
