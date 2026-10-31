using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MiniSupermarket.API.Models
{
    /// <summary>
    /// Thực thể phân loại nhóm ngành hàng trong siêu thị mini
    /// Ánh xạ tương ứng với bảng [Categories] trong SQL Server
    /// </summary>
    [Table("Categories")]
    public class Category
    {
        // 1. Khóa chính tự tăng IDENTITY(1,1)
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryId { get; set; }

        // 2. Tên nhóm hàng: Bắt buộc, tối đa 100 ký tự
        [Required(ErrorMessage = "Tên nhóm hàng không được để trống!")]
        [StringLength(100, ErrorMessage = "Tên nhóm hàng không vượt quá 100 ký tự!")]
        public string CategoryName { get; set; } = string.Empty;

        // 3. Mô tả chi tiết nhóm hàng (Cho phép null)
        [StringLength(255)]
        public string? Description { get; set; }

        // Thuộc tính điều hướng quan hệ 1-Nhiều (1 Nhóm hàng chứa nhiều Sản phẩm)
        // [JsonIgnore] ngăn ngừa lỗi lặp vòng vô hạn (Circular Reference) khi serialize JSON
        [JsonIgnore]
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
