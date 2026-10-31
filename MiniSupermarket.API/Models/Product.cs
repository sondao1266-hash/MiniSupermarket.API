using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MiniSupermarket.API.Models
{
    /// <summary>
    /// Thực thể quản lý chi tiết sản phẩm, giá bán lẻ và số lượng tồn kho
    /// Ánh xạ tương ứng với bảng [Products] trong SQL Server
    /// </summary>
    [Table("Products")]
    public class Product
    {
        // 1. Khóa chính tự tăng IDENTITY(1,1)
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }

        // 2. Mã vạch Barcode để quét tại quầy thu ngân POS: Duy nhất, tối đa 50 ký tự
        [Required(ErrorMessage = "Mã vạch không được để trống!")]
        [StringLength(50, ErrorMessage = "Mã vạch không vượt quá 50 ký tự!")]
        public string Barcode { get; set; } = string.Empty;

        // 3. Tên sản phẩm hàng hóa
        [Required(ErrorMessage = "Tên sản phẩm không được để trống!")]
        [StringLength(150, ErrorMessage = "Tên sản phẩm không vượt quá 150 ký tự!")]
        public string ProductName { get; set; } = string.Empty;

        // 4. Giá bán lẻ niêm yết: Kiểu tiền tệ chính xác decimal(18,2)
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá bán phải lớn hơn hoặc bằng 0!")]
        public decimal Price { get; set; }

        // 5. Số lượng hàng hóa hiện còn trong kho
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn kho không được âm!")]
        public int StockQuantity { get; set; }

        // 6. Khóa ngoại tham chiếu đến bảng Categories (CategoryId)
        [Required]
        public int CategoryId { get; set; }

        // Thuộc tính điều hướng trỏ về đối tượng nhóm hàng cha
        [ForeignKey(nameof(CategoryId))]
        public virtual Category? Category { get; set; }

        // Danh sách chi tiết hóa đơn mà sản phẩm này xuất hiện
        [JsonIgnore]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        // Bài tập 2 trang 20: Cảnh báo hàng sắp hết (IsLowStock)
        [NotMapped]
        public bool IsLowStock => StockQuantity <= 20;
    }
}
