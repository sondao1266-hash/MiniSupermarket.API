/*
 * Mo ta: Controller ADO.NET thuan - Tra cuu ma vach sieu toc (SqlDataReader),
 *        demo DataTable (Disconnected) va 2 bai tap mo rong cua Tiet 2.
 */
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace MiniSupermarket.API.Controllers
{
    // DTO tra cuu san pham cho quay POS
    public class ProductLookupDto
    {
        public int ProductId { get; set; }
        public string Barcode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }

    [Route("api/ado-products")]
    [ApiController]
    public class AdoProductController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AdoProductController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string ConnStr => _configuration.GetConnectionString("DefaultConnection")!;

        private static ProductLookupDto MapReader(SqlDataReader reader) => new ProductLookupDto
        {
            ProductId = Convert.ToInt32(reader["ProductId"]),
            Barcode = reader["Barcode"].ToString() ?? string.Empty,
            ProductName = reader["ProductName"].ToString() ?? string.Empty,
            Price = Convert.ToDecimal(reader["Price"]),
            StockQuantity = Convert.ToInt32(reader["StockQuantity"])
        };

        // =====================================================================
        // THAO TAC 1: TRA CUU THEO MA VACH (CONNECTED MODE - SqlDataReader)
        // GET: api/ado-products/barcode/893000000001
        // =====================================================================
        [HttpGet("barcode/{barcode}")]
        public IActionResult GetProductByBarcode(string barcode)
        {
            if (string.IsNullOrWhiteSpace(barcode))
                return BadRequest(new { message = "Mã vạch không được để trống!" });

            const string query = @"SELECT ProductId, Barcode, ProductName, Price, StockQuantity
                                   FROM Products
                                   WHERE Barcode = @Barcode";

            try
            {
                using (var connection = new SqlConnection(ConnStr))
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Barcode", barcode.Trim());
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                            return Ok(MapReader(reader));
                    }
                }
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new { message = "Lỗi truy vấn SQL: " + ex.Message });
            }

            return NotFound(new { message = $"Không tìm thấy sản phẩm có mã vạch: {barcode}" });
        }

        // =====================================================================
        // THAO TAC 2: TAI DANH SACH LUU DEM (DISCONNECTED - DataTable + Adapter)
        // GET: api/ado-products/disconnected-cache
        // =====================================================================
        [HttpGet("disconnected-cache")]
        public IActionResult GetProductsDisconnected()
        {
            var productTable = new DataTable("CachedProducts");

            try
            {
                using (var connection = new SqlConnection(ConnStr))
                using (var command = new SqlCommand(
                    @"SELECT ProductId, Barcode, ProductName, Price, StockQuantity
                      FROM Products ORDER BY ProductId ASC", connection))
                using (var adapter = new SqlDataAdapter(command))
                {
                    // Fill(): tu Open -> do du lieu -> tu Close ket noi
                    adapter.Fill(productTable);
                }
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new { message = "Lỗi truy vấn SQL: " + ex.Message });
            }

            // Tu day ket noi da dong hoan toan, thao tac tren RAM
            var resultList = new List<ProductLookupDto>();
            foreach (DataRow row in productTable.Rows)
            {
                resultList.Add(new ProductLookupDto
                {
                    ProductId = Convert.ToInt32(row["ProductId"]),
                    Barcode = row["Barcode"].ToString() ?? string.Empty,
                    ProductName = row["ProductName"].ToString() ?? string.Empty,
                    Price = Convert.ToDecimal(row["Price"]),
                    StockQuantity = Convert.ToInt32(row["StockQuantity"])
                });
            }

            return Ok(new
            {
                totalLoaded = productTable.Rows.Count,
                cachedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                data = resultList
            });
        }

        // =====================================================================
        // BAI TAP 2.1: San pham ton kho thap (SqlDataReader)
        // GET: api/ado-products/filter-stock?minStock=50
        // (Ten tham so giu theo de bai; y nghia: ton kho <= gia tri truyen vao)
        // =====================================================================
        [HttpGet("filter-stock")]
        public IActionResult FilterStock([FromQuery] int minStock = 50)
        {
            const string query = @"SELECT ProductId, Barcode, ProductName, Price, StockQuantity
                                   FROM Products
                                   WHERE StockQuantity <= @MinStock
                                   ORDER BY StockQuantity ASC";
            var list = new List<ProductLookupDto>();

            try
            {
                using (var connection = new SqlConnection(ConnStr))
                using (var cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@MinStock", minStock);
                    connection.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read()) list.Add(MapReader(reader));
                    }
                }
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new { message = "Lỗi truy vấn SQL: " + ex.Message });
            }

            return Ok(new { threshold = minStock, count = list.Count, data = list });
        }

        // =====================================================================
        // BAI TAP 2.2: Dem san pham theo danh muc (DataTable + SqlDataAdapter)
        // GET: api/ado-products/count-by-category
        // =====================================================================
        [HttpGet("count-by-category")]
        public IActionResult CountByCategory()
        {
            var table = new DataTable();

            try
            {
                using (var connection = new SqlConnection(ConnStr))
                using (var adapter = new SqlDataAdapter(
                    @"SELECT p.CategoryId, c.CategoryName, COUNT(*) AS Total
                      FROM Products p
                      JOIN Categories c ON c.CategoryId = p.CategoryId
                      GROUP BY p.CategoryId, c.CategoryName
                      ORDER BY p.CategoryId", connection))
                {
                    adapter.Fill(table);
                }
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new { message = "Lỗi truy vấn SQL: " + ex.Message });
            }

            var result = new List<object>();
            foreach (DataRow row in table.Rows)
            {
                result.Add(new
                {
                    categoryId = Convert.ToInt32(row["CategoryId"]),
                    categoryName = row["CategoryName"].ToString(),
                    total = Convert.ToInt32(row["Total"])
                });
            }

            return Ok(result);
        }
    }
}
