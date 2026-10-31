using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MiniSupermarket.API.Models
{
    /// <summary>
    /// Thực thể lưu chi tiết từng mặt hàng có trong một hóa đơn bán lẻ
    /// Ánh xạ tương ứng với bảng [OrderDetails] trong SQL Server
    /// </summary>
    [Table("OrderDetails")]
    public class OrderDetail
    {
        // 1. Khóa chính tự tăng
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderDetailId { get; set; }

        // 2. Khóa ngoại tham chiếu đến hóa đơn (Orders.OrderId)
        [Required]
        public int OrderId { get; set; }

        // 3. Khóa ngoại tham chiếu đến sản phẩm (Products.ProductId)
        [Required]
        public int ProductId { get; set; }

        // 4. Số lượng mặt hàng mua (Tối thiểu là 1)
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng mua tối thiểu là 1!")]
        public int Quantity { get; set; }

        // 5. Đơn giá bán tại thời điểm xuất hóa đơn
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        // 6. Thành tiền dòng sản phẩm (LineTotal = Quantity * UnitPrice)
        [Column(TypeName = "decimal(18,2)")]
        public decimal LineTotal { get; set; }

        // Thuộc tính điều hướng về hóa đơn cha
        [ForeignKey(nameof(OrderId))]
        [JsonIgnore]
        public virtual Order? Order { get; set; }

        // Thuộc tính điều hướng để truy xuất thông tin sản phẩm (Tên, mã vạch...)
        [ForeignKey(nameof(ProductId))]
        public virtual Product? Product { get; set; }
    }
}
