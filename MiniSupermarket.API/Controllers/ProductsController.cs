using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniSupermarket.API.Data;
using MiniSupermarket.API.Models;

namespace MiniSupermarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly SupermarketDbContext _context;

        public ProductsController(SupermarketDbContext context)
        {
            _context = context;
        }

        // =========================================================================
        // GET: api/products/paged?keyword=sua&categoryId=3&pageNumber=1&pageSize=5
        // BUỔI 6: TIẾT 1 & 2 + BÀI TẬP 6.2 (BỘ LỌC CATEGORY)
        // =========================================================================
        [HttpGet("paged")]
        public async Task<IActionResult> GetPagedProducts(
            [FromQuery] string? keyword,
            [FromQuery] int? categoryId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 5)
        {
            // 1. Kiểm tra tham số trang đầu vào hợp lệ
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 50) pageSize = 5;

            // 2. Khởi tạo truy vấn LINQ IQueryable kết hợp Eager Loading (.Include)
            // AsNoTracking giúp tối ưu hóa bộ nhớ RAM cho các truy vấn chỉ đọc dữ liệu
            var query = _context.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .AsQueryable();

            // 3. Lọc theo từ khóa tìm kiếm (nếu người dùng có nhập)
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                string search = keyword.Trim().ToLower();
                query = query.Where(p => p.ProductName.ToLower().Contains(search)
                                       || p.Barcode.Contains(search));
            }

            // Bài tập 6.2: Lọc theo nhóm hàng (nếu có chọn)
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            // 4. Đếm tổng số lượng bản ghi thỏa mãn điều kiện
            int totalItems = await query.CountAsync();

            // 5. Tính toán tổng số trang
            int totalPages = (int)Math.Ceiling((double)totalItems / (double)pageSize);
            if (totalPages == 0) totalPages = 1;

            // 6. Áp dụng toán tử Skip và Take để phân trang trực tiếp dưới SQL Server
            var items = await query
                .OrderBy(p => p.ProductId) // Bắt buộc phải OrderBy trước khi Skip/Take
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductItemDto
                {
                    ProductId = p.ProductId,
                    Barcode = p.Barcode,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category != null ? p.Category.CategoryName : string.Empty
                })
                .ToListAsync();

            // 7. Đóng gói kết quả trả về cho Client
            var response = new PagedResult<ProductItemDto>
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                Items = items
            };

            return Ok(response);
        }

        // =========================================================================
        // GET: api/products/with-category
        // PHỤ LỤC: BÀI TẬP 1 (Eager Loading .Include(p => p.Category) + AsNoTracking)
        // =========================================================================
        [HttpGet("with-category")]
        public async Task<IActionResult> GetProductsWithCategory()
        {
            var list = await _context.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .Select(p => new ProductItemDto
                {
                    ProductId = p.ProductId,
                    Barcode = p.Barcode,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category != null ? p.Category.CategoryName : string.Empty
                })
                .ToListAsync();

            return Ok(list);
        }

        // GET api/products
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? keyword, [FromQuery] int? categoryId)
        {
            var query = _context.Products.Include(p => p.Category).AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                string search = keyword.Trim().ToLower();
                query = query.Where(p => p.Barcode.Contains(search) || p.ProductName.ToLower().Contains(search));
            }

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            var list = await query
                .OrderBy(p => p.ProductId)
                .Select(p => new ProductItemDto
                {
                    ProductId = p.ProductId,
                    Barcode = p.Barcode,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category != null ? p.Category.CategoryName : ""
                })
                .ToListAsync();

            return Ok(list);
        }

        // GET api/products/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var p = await _context.Products
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x => x.ProductId == id);

            if (p == null) return NotFound(new { message = "Không tìm thấy sản phẩm!" });

            return Ok(new ProductItemDto
            {
                ProductId = p.ProductId,
                Barcode = p.Barcode,
                ProductName = p.ProductName,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                CategoryId = p.CategoryId,
                CategoryName = p.Category != null ? p.Category.CategoryName : ""
            });
        }

        // GET api/products/barcode/{barcode}
        [HttpGet("barcode/{barcode}")]
        public async Task<IActionResult> GetByBarcode(string barcode)
        {
            var p = await _context.Products
                .Include(x => x.Category)
                .Where(x => x.Barcode == barcode)
                .Select(x => new ProductItemDto
                {
                    ProductId = x.ProductId,
                    Barcode = x.Barcode,
                    ProductName = x.ProductName,
                    Price = x.Price,
                    StockQuantity = x.StockQuantity,
                    CategoryId = x.CategoryId,
                    CategoryName = x.Category != null ? x.Category.CategoryName : ""
                })
                .FirstOrDefaultAsync();

            if (p == null) return NotFound(new { message = "Không tìm thấy sản phẩm có mã vạch này!" });
            return Ok(p);
        }

        // POST api/products
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductSaveDto dto)
        {
            var error = await ValidateAsync(dto, null);
            if (error != null) return BadRequest(new { message = error });

            var product = new Product
            {
                Barcode = dto.Barcode.Trim(),
                ProductName = dto.ProductName.Trim(),
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                CategoryId = dto.CategoryId
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return Ok(new { productId = product.ProductId, message = "Thêm mới sản phẩm thành công!" });
        }

        // PUT api/products/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductSaveDto dto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound(new { message = "Không tìm thấy sản phẩm!" });

            var error = await ValidateAsync(dto, id);
            if (error != null) return BadRequest(new { message = error });

            product.Barcode = dto.Barcode.Trim();
            product.ProductName = dto.ProductName.Trim();
            product.Price = dto.Price;
            product.StockQuantity = dto.StockQuantity;
            product.CategoryId = dto.CategoryId;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật sản phẩm thành công!" });
        }

        // DELETE api/products/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound(new { message = "Không tìm thấy sản phẩm!" });

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Đã xóa sản phẩm thành công!" });
        }

        private async Task<string?> ValidateAsync(ProductSaveDto dto, int? currentId)
        {
            if (string.IsNullOrWhiteSpace(dto.Barcode)) return "Mã vạch không được trống!";
            if (string.IsNullOrWhiteSpace(dto.ProductName)) return "Tên sản phẩm không được trống!";
            if (dto.Price < 0) return "Đơn giá không được âm!";
            if (dto.StockQuantity < 0) return "Số lượng tồn không được âm!";

            if (!await _context.Categories.AnyAsync(c => c.CategoryId == dto.CategoryId))
                return "Nhóm hàng không tồn tại!";

            var barcode = dto.Barcode.Trim();
            if (await _context.Products.AnyAsync(p => p.Barcode == barcode && p.ProductId != currentId))
                return "Mã vạch đã tồn tại!";

            return null;
        }
    }

    public class ProductSaveDto
    {
        public string Barcode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int CategoryId { get; set; }
    }
}
