/*
 * Ho ten sinh vien: Nguyen Van Teo
 * Ma sinh vien: 123456789
 * Ngay tao: 15/09/2026
 * Mo ta: Controller ADO.NET thuc thi Stored Procedure xuat bao cao ca ban
 */
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MiniSupermarket.API.Data;
using MiniSupermarket.API.Models;
using System.Data;

namespace MiniSupermarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly SupermarketDbContext _context;

        // Dependency Injection lay chuoi ket noi va DbContext
        public ReportsController(IConfiguration configuration, SupermarketDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        private string ConnStr => _configuration.GetConnectionString("DefaultConnection")!;

        // =========================================================================
        // GET: api/reports/revenue-by-cashier
        // Chuc nang: Goi Stored Procedure sp_GetRevenueByCashier qua SqlDataReader
        // =========================================================================
        [HttpGet("revenue-by-cashier")]
        public IActionResult GetRevenueByCashier()
        {
            var reportList = new List<CashierRevenueDto>();

            // 1. Khoi tao SqlConnection trong khoi using dam bao tu dong dong ket noi
            using (SqlConnection connection = new SqlConnection(ConnStr))
            {
                // 2. Gan CommandText la CHINH XAC ten Stored Procedure tren SQL Server
                using (SqlCommand command = new SqlCommand("sp_GetRevenueByCashier", connection))
                {
                    // 3. DIEM KHOA HOC QUAN TRONG: Thiet lap CommandType la StoredProcedure
                    // Neu thieu dong nay, SQL Server se hieu nham day la chuoi SQL thuong va bao loi cu phap!
                    command.CommandType = CommandType.StoredProcedure;

                    try
                    {
                        // 4. Mo ket noi vat ly toi SQL Server
                        connection.Open();

                        // 5. Thuc thi bang ExecuteReader(): Nhan ve luong du lieu forward-only
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // 6. Vong lap doc tung dong ket qua cua Stored Procedure
                            while (reader.Read())
                            {
                                // Anh xa tung cot du lieu tu reader vao doi tuong DTO
                                var item = new CashierRevenueDto
                                {
                                    CashierUsername = reader["CashierUsername"].ToString() ?? string.Empty,
                                    TotalOrders = Convert.ToInt32(reader["TotalOrders"]),
                                    TotalRevenue = Convert.ToDecimal(reader["TotalRevenue"]),
                                    TotalPointsAwarded = reader["TotalPointsAwarded"] != DBNull.Value
                                        ? Convert.ToInt32(reader["TotalPointsAwarded"])
                                        : 0
                                };
                                reportList.Add(item);
                            }
                        } // reader tu dong Close() tai day
                    }
                    catch (SqlException ex)
                    {
                        // Bat loi neu quen tao Stored Procedure tren CSDL hoac sai chuoi ket noi
                        return StatusCode(500, new
                        {
                            success = false,
                            message = "Lỗi thực thi Stored Procedure trên SQL Server: " + ex.Message
                        });
                    }
                }
            } // connection tu dong Close() va Dispose() tai day

            // 7. Tra ve ma 200 OK kem danh sach bao cao da tong hop
            return Ok(new
            {
                success = true,
                generatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                totalCashiers = reportList.Count,
                grandTotalRevenue = reportList.Sum(x => x.TotalRevenue),
                data = reportList
            });
        }

        // =========================================================================
        // GET: api/reports/revenue-by-date?from=2026-10-01&to=2026-10-31
        // Bai tap 3.1: Stored Procedure sp_GetRevenueByDateRange (co tham so)
        // =========================================================================
        [HttpGet("revenue-by-date")]
        public IActionResult GetRevenueByDateRange([FromQuery(Name = "from")] DateTime fromDate, [FromQuery(Name = "to")] DateTime toDate)
        {
            if (toDate < fromDate)
            {
                return BadRequest(new { message = "Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu!" });
            }

            DateTime toEnd = toDate.TimeOfDay == TimeSpan.Zero ? toDate.Date.AddDays(1).AddTicks(-1) : toDate;
            var reportList = new List<CashierRevenueDto>();

            try
            {
                using (var connection = new SqlConnection(ConnStr))
                using (var command = new SqlCommand("sp_GetRevenueByDateRange", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@FromDate", fromDate);
                    command.Parameters.AddWithValue("@ToDate", toEnd);
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reportList.Add(new CashierRevenueDto
                            {
                                CashierUsername = reader["CashierUsername"].ToString() ?? string.Empty,
                                TotalOrders = Convert.ToInt32(reader["TotalOrders"]),
                                TotalRevenue = Convert.ToDecimal(reader["TotalRevenue"]),
                                TotalPointsAwarded = reader["TotalPointsAwarded"] != DBNull.Value
                                    ? Convert.ToInt32(reader["TotalPointsAwarded"])
                                    : 0
                            });
                        }
                    }
                }

                return Ok(new
                {
                    success = true,
                    fromDate = fromDate,
                    toDate = toEnd,
                    totalCashiers = reportList.Count,
                    grandTotalRevenue = reportList.Sum(x => x.TotalRevenue),
                    data = reportList
                });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi thực thi Stored Procedure: " + ex.Message });
            }
        }

        // =========================================================================
        // GET: api/reports/top5-products
        // Bai tap 3.2: Stored Procedure sp_GetTop5SellingProducts (Top 5 ban chay nhat)
        // =========================================================================
        [HttpGet("top5-products")]
        public IActionResult GetTop5Products()
        {
            var list = new List<TopProductDto>();
            try
            {
                using (var connection = new SqlConnection(ConnStr))
                using (var command = new SqlCommand("sp_GetTop5SellingProducts", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new TopProductDto
                            {
                                ProductId = Convert.ToInt32(reader["ProductId"]),
                                ProductName = reader["ProductName"].ToString() ?? string.Empty,
                                TotalSold = Convert.ToInt32(reader["TotalSold"]),
                                TotalRevenue = Convert.ToDecimal(reader["TotalRevenue"])
                            });
                        }
                    }
                }

                return Ok(new
                {
                    success = true,
                    data = list
                });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi thực thi Stored Procedure: " + ex.Message });
            }
        }

        // =========================================================================
        // GET: api/reports/daily?date=2026-10-23
        // Chức năng: Báo cáo nhanh cho màn hình FormQuickReport (Tổng đơn, Tổng doanh thu, Mặt hàng bán chạy nhất)
        // =========================================================================
        [HttpGet("daily")]
        public async Task<IActionResult> GetDailyReport([FromQuery] DateTime? date)
        {
            DateTime target = (date ?? DateTime.Today).Date;
            DateTime next = target.AddDays(1);

            var ordersInDay = await _context.Orders
                .Where(o => o.CreatedDate >= target && o.CreatedDate < next)
                .ToListAsync();

            int totalOrders = ordersInDay.Count;
            decimal totalRevenue = ordersInDay.Sum(o => o.FinalAmount);

            string bestSeller = "";
            if (totalOrders > 0)
            {
                var topProduct = await (from d in _context.OrderDetails
                                        join o in _context.Orders on d.OrderId equals o.OrderId
                                        where o.CreatedDate >= target && o.CreatedDate < next
                                        join p in _context.Products on d.ProductId equals p.ProductId
                                        group d by p.ProductName into g
                                        orderby g.Sum(x => x.Quantity) descending
                                        select g.Key)
                                        .FirstOrDefaultAsync();

                bestSeller = topProduct ?? "Không có chi tiết";
            }
            else
            {
                // Fallback: Tìm sản phẩm bán chạy nhất hệ thống để hiển thị thông tin tham khảo
                var overallTop = await (from d in _context.OrderDetails
                                        join p in _context.Products on d.ProductId equals p.ProductId
                                        group d by p.ProductName into g
                                        orderby g.Sum(x => x.Quantity) descending
                                        select g.Key)
                                        .FirstOrDefaultAsync();

                bestSeller = overallTop != null 
                    ? $"{overallTop} (Toàn hệ thống)" 
                    : "Chưa có dữ liệu";
            }

            return Ok(new
            {
                totalOrders = totalOrders,
                totalRevenue = totalRevenue,
                bestSeller = bestSeller
            });
        }
    }
}
