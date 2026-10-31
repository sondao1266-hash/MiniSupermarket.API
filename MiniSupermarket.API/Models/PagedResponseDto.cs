/*
 * Họ tên sinh viên: Nguyen Van Teo
 * Mã sinh viên: 123456789
 * Mô tả: Lớp DTO tổng quát (Generic) chứa dữ liệu phân trang trả về cho Client
 */
namespace MiniSupermarket.API.Models
{
    // DTO chi tiết từng sản phẩm trả về kèm tên nhóm hàng
    public class ProductItemDto
    {
        public int ProductId { get; set; }
        public string Barcode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty; // Lấy từ bảng Categories qua Eager Loading
    }

    // DTO bao bọc kết quả phân trang
    public class PagedResult<T>
    {
        public int CurrentPage { get; set; }       // Trang hiện tại (1, 2, 3...)
        public int PageSize { get; set; }          // Số phần tử trên mỗi trang (ví dụ: 5 dòng)
        public int TotalItems { get; set; }         // Tổng số sản phẩm thỏa mãn điều kiện
        public int TotalPages { get; set; }         // Tổng số trang = Ceiling(TotalItems / PageSize)
        public List<T> Items { get; set; } = new(); // Danh sách dữ liệu của trang hiện hành
    }
}
