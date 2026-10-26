using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniSupermarket.API.Data;
using MiniSupermarket.API.Models;

namespace MiniSupermarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly SupermarketDbContext _context;

        public OrdersController(SupermarketDbContext context)
        {
            _context = context;
        }

        // GET api/orders
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _context.Orders
                .Include(o => o.OrderDetails)
                .OrderByDescending(o => o.OrderId)
                .AsNoTracking()
                .ToListAsync();
            return Ok(list);
        }

        // GET api/orders/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(d => d.Product)
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null) return NotFound(new { message = "Không tìm thấy hóa đơn!" });
            return Ok(order);
        }

        // POST api/orders/checkout (EF Core version)
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequest req)
        {
            if (req.Items == null || req.Items.Count == 0)
                return BadRequest(new { message = "Giỏ hàng đang trống!" });

            var grouped = req.Items
                .GroupBy(i => i.ProductId)
                .Select(g => new { ProductId = g.Key, Quantity = g.Sum(x => x.Quantity) })
                .ToList();

            if (grouped.Any(g => g.Quantity <= 0))
                return BadRequest(new { message = "Số lượng phải lớn hơn 0!" });

            var ids = grouped.Select(g => g.ProductId).ToList();
            var products = await _context.Products.Where(p => ids.Contains(p.ProductId)).ToListAsync();
            if (products.Count != ids.Count)
                return BadRequest(new { message = "Có sản phẩm trong giỏ không còn tồn tại!" });

            string orderCode = $"HD-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}";
            int totalItems = grouped.Sum(x => x.Quantity);
            decimal total = 0;

            var details = new List<OrderDetail>();
            foreach (var g in grouped)
            {
                var p = products.First(x => x.ProductId == g.ProductId);

                if (p.StockQuantity < g.Quantity)
                    return BadRequest(new { message = $"Sản phẩm \"{p.ProductName}\" chỉ còn {p.StockQuantity} trong kho!" });

                decimal line = p.Price * g.Quantity;
                p.StockQuantity -= g.Quantity;
                total += line;

                details.Add(new OrderDetail
                {
                    ProductId = p.ProductId,
                    Quantity = g.Quantity,
                    UnitPrice = p.Price,
                    LineTotal = line
                });
            }

            int rewardPoints = (int)(total / 10000m);
            int customerId = req.CustomerId ?? 15; // Mặc định khách vãng lai nếu không chọn

            var order = new Order
            {
                OrderCode = orderCode,
                CreatedDate = DateTime.Now,
                CashierUsername = !string.IsNullOrWhiteSpace(req.CashierUsername) ? req.CashierUsername : (User.Identity?.Name ?? "cashier01"),
                CustomerId = customerId,
                TotalItems = totalItems,
                FinalAmount = total,
                RewardPoints = rewardPoints,
                OrderDetails = details
            };

            int? updatedCustomerPoints = null;
            string? updatedRank = null;

            if (req.CustomerId.HasValue)
            {
                var customer = await _context.Customers.FindAsync(req.CustomerId.Value);
                if (customer != null)
                {
                    if (rewardPoints > 0)
                    {
                        customer.RewardPoints += rewardPoints;
                        if (customer.RewardPoints >= 500) customer.MembershipRank = "Bạch Kim";
                        else if (customer.RewardPoints >= 200) customer.MembershipRank = "Vàng";
                        else if (customer.RewardPoints >= 100) customer.MembershipRank = "Bạc";
                    }
                    updatedCustomerPoints = customer.RewardPoints;
                    updatedRank = customer.MembershipRank;
                }
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            decimal changeAmount = req.CashReceived >= total ? req.CashReceived - total : 0;

            return Ok(new
            {
                success = true,
                orderId = order.OrderId,
                orderCode = order.OrderCode,
                totalItems = order.TotalItems,
                totalAmount = order.FinalAmount,
                finalAmount = order.FinalAmount,
                cashReceived = req.CashReceived,
                changeAmount = changeAmount,
                pointsEarned = rewardPoints,
                customerPoints = updatedCustomerPoints,
                membershipRank = updatedRank
            });
        }
    }

    public class CheckoutRequest
    {
        public string CashierUsername { get; set; } = string.Empty;
        public int? CustomerId { get; set; }
        public decimal CashReceived { get; set; }
        public List<CheckoutItem> Items { get; set; } = new List<CheckoutItem>();
    }

    public class CheckoutItem
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
