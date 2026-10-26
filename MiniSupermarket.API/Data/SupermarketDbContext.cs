using System;
using Microsoft.EntityFrameworkCore;
using MiniSupermarket.API.Models;

namespace MiniSupermarket.API.Data
{
    /// <summary>
    /// Ngữ cảnh cơ sở dữ liệu đại diện cho phiên làm việc với SQL Server
    /// Đề tài: Cửa hàng Đồ gia dụng Nhà bếp Kitchen Express
    /// </summary>
    public class SupermarketDbContext : DbContext
    {
        public SupermarketDbContext(DbContextOptions<SupermarketDbContext> options) :
            base(options) { }

        // Khai báo 6 bảng dữ liệu hệ thống
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =========================================================================
            // A. CẤU HÌNH RÀNG BUỘC CHỈ MỤC DUY NHẤT (UNIQUE INDEX)
            // =========================================================================
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
            modelBuilder.Entity<Category>().HasIndex(c => c.CategoryName).IsUnique();
            modelBuilder.Entity<Product>().HasIndex(p => p.Barcode).IsUnique();
            modelBuilder.Entity<Customer>().HasIndex(c => c.PhoneNumber).IsUnique();
            modelBuilder.Entity<Order>().HasIndex(o => o.OrderCode).IsUnique();

            // =========================================================================
            // B. DATA SEEDING CHO 6 BẢNG - ĐỀ TÀI: KITCHEN EXPRESS (GIA DỤNG NHÀ BẾP)
            // =========================================================================

            // 1. DATA SEEDING: BẢNG USERS (15 Nhân viên thuộc 3 bộ phận: Admin, Cashier, Warehouse)
            modelBuilder.Entity<User>().HasData(
                new User { UserId = 1, Username = "admin", PasswordHash = "admin@123", FullName = "Nguyễn Quản Trị - Kitchen Express", Role = "Admin", IsActive = true },
                new User { UserId = 2, Username = "manager01", PasswordHash = "manager@123", FullName = "Trần Cửa Hàng Trưởng", Role = "Admin", IsActive = true },
                new User { UserId = 3, Username = "cashier01", PasswordHash = "cashier@123", FullName = "Lê Thu Ngân Quầy 1", Role = "Cashier", IsActive = true },
                new User { UserId = 4, Username = "cashier02", PasswordHash = "cashier@123", FullName = "Phạm Thu Ngân Quầy 2", Role = "Cashier", IsActive = true },
                new User { UserId = 5, Username = "cashier03", PasswordHash = "cashier@123", FullName = "Hoàng Thu Ngân Quầy 3", Role = "Cashier", IsActive = true },
                new User { UserId = 6, Username = "cashier04", PasswordHash = "cashier@123", FullName = "Vũ Thu Ngân Quầy 4", Role = "Cashier", IsActive = true },
                new User { UserId = 7, Username = "cashier05", PasswordHash = "cashier@123", FullName = "Đỗ Bán Hàng Thiết Bị", Role = "Cashier", IsActive = true },
                new User { UserId = 8, Username = "cashier06", PasswordHash = "cashier@123", FullName = "Ngô Tư Vấn Gia Dụng", Role = "Cashier", IsActive = true },
                new User { UserId = 9, Username = "cashier07", PasswordHash = "cashier@123", FullName = "Bùi Bán Lẻ Ca Tối", Role = "Cashier", IsActive = true },
                new User { UserId = 10, Username = "warehouse01", PasswordHash = "wh@123", FullName = "Đặng Quản Lý Kho Bếp", Role = "Warehouse", IsActive = true },
                new User { UserId = 11, Username = "warehouse02", PasswordHash = "wh@123", FullName = "Trương Thủ Kho Thiết Bị", Role = "Warehouse", IsActive = true },
                new User { UserId = 12, Username = "warehouse03", PasswordHash = "wh@123", FullName = "Lý Kiểm Kê Hàng Nhập", Role = "Warehouse", IsActive = true },
                new User { UserId = 13, Username = "cashier08", PasswordHash = "cashier@123", FullName = "Mai Bán Hàng Trực Tuyến", Role = "Cashier", IsActive = true },
                new User { UserId = 14, Username = "cashier09", PasswordHash = "cashier@123", FullName = "Dương Thu Ngân Cuối Tuần", Role = "Cashier", IsActive = true },
                new User { UserId = 15, Username = "staff_lock", PasswordHash = "lock@123", FullName = "Hồ Nhân Viên Nghỉ Việc", Role = "Cashier", IsActive = false }
            );

            // 2. DATA SEEDING: BẢNG CATEGORIES (15 Nhóm đồ gia dụng nhà bếp Kitchen Express)
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, CategoryName = "Nồi & Chảo chống dính", Description = "Chảo từ, chảo sâu lòng, nồi inox 3 đáy, 5 đáy" },
                new Category { CategoryId = 2, CategoryName = "Thiết bị Điện nhà bếp", Description = "Bếp từ, nồi cơm điện cao tần, ấm đun giữ nhiệt" },
                new Category { CategoryId = 3, CategoryName = "Dụng cụ Cắt thái & Sơ chế", Description = "Bộ dao thép không gỉ, thớt gỗ kháng khuẩn, kéo bếp" },
                new Category { CategoryId = 4, CategoryName = "Hộp đựng & Bảo quản thực phẩm", Description = "Hộp thủy tinh chịu nhiệt, hộp cơm giữ nhiệt văn phòng" },
                new Category { CategoryId = 5, CategoryName = "Dụng cụ Pha chế & Bàn ăn", Description = "Bình giữ nhiệt inox 304, bộ ấm chén gốm sứ Bát Tràng" },
                new Category { CategoryId = 6, CategoryName = "Lò vi sóng & Thiết bị nướng", Description = "Lò vi sóng điện tử, lò nướng thùng, vỉ nướng chống dính" },
                new Category { CategoryId = 7, CategoryName = "Nồi chiên & Nồi áp suất", Description = "Nồi chiên không dầu công nghệ Rapid Air, nồi áp suất điện" },
                new Category { CategoryId = 8, CategoryName = "Máy xay & Máy ép hoa quả", Description = "Máy xay sinh tố đa năng, máy ép chậm nước trái cây" },
                new Category { CategoryId = 9, CategoryName = "Thiết bị Pha trà & Cà phê", Description = "Bình đun siêu tốc thông minh, phin pha, máy pha cà phê mini" },
                new Category { CategoryId = 10, CategoryName = "Dụng cụ Làm bánh & Định lượng", Description = "Cân tiểu ly điện tử, khuôn bánh silicon, cây đánh trứng" },
                new Category { CategoryId = 11, CategoryName = "Kệ giá & Khay để đồ nhà bếp", Description = "Kệ để dao thớt inox 304, giá úp bát đĩa thông minh" },
                new Category { CategoryId = 12, CategoryName = "Dụng cụ Vệ sinh nhà bếp", Description = "Cọ xoong nồi đa năng, giá treo khăn lau, găng tay silicon" },
                new Category { CategoryId = 13, CategoryName = "Đồ sành sứ & Thủy tinh cao cấp", Description = "Bát đĩa sứ xương, tô canh thủy tinh cường lực, ly thủy tinh" },
                new Category { CategoryId = 14, CategoryName = "Đũa, Muỗng & Dao muỗng nĩa", Description = "Bộ muỗng nĩa inox mạ vàng, đũa gỗ bu-mê, muỗng canh silicon" },
                new Category { CategoryId = 15, CategoryName = "Vật tư & Phụ kiện Kitchen Express", Description = "Giấy nến nồi chiên, màng bọc thực phẩm, túi zip đóng kín" }
            );

            // 3. DATA SEEDING: BẢNG PRODUCTS (20 Mặt hàng gia dụng nhà bếp Kitchen Express)
            modelBuilder.Entity<Product>().HasData(
                new Product { ProductId = 1, Barcode = "893000000001", ProductName = "Nồi chiên không dầu Philips 5L", Price = 2490000m, StockQuantity = 25, CategoryId = 7 },
                new Product { ProductId = 2, Barcode = "893000000002", ProductName = "Máy xay sinh tố đa năng Sunhouse", Price = 650000m, StockQuantity = 40, CategoryId = 8 },
                new Product { ProductId = 3, Barcode = "893000000003", ProductName = "Nồi cơm điện cao tần Panasonic 1.8L", Price = 1850000m, StockQuantity = 20, CategoryId = 2 },
                new Product { ProductId = 4, Barcode = "893000000004", ProductName = "Lò vi sóng điện tử Sharp 20L", Price = 1790000m, StockQuantity = 15, CategoryId = 6 },
                new Product { ProductId = 5, Barcode = "893000000005", ProductName = "Ấm siêu tốc giữ nhiệt Lock&Lock 1.7L", Price = 520000m, StockQuantity = 60, CategoryId = 9 },
                new Product { ProductId = 6, Barcode = "893000000006", ProductName = "Bộ nồi inox 5 đáy Elmich 3 chiếc", Price = 1250000m, StockQuantity = 30, CategoryId = 1 },
                new Product { ProductId = 7, Barcode = "893000000007", ProductName = "Chảo chống dính sâu lòng Tefal 28cm", Price = 480000m, StockQuantity = 55, CategoryId = 1 },
                new Product { ProductId = 8, Barcode = "893000000008", ProductName = "Nồi áp suất điện đa năng Sunhouse 5L", Price = 890000m, StockQuantity = 25, CategoryId = 7 },
                new Product { ProductId = 9, Barcode = "893000000009", ProductName = "Bộ dao bếp cao cấp 6 món thép không gỉ", Price = 450000m, StockQuantity = 35, CategoryId = 3 },
                new Product { ProductId = 10, Barcode = "893000000010", ProductName = "Thớt gỗ Teak kháng khuẩn cao cấp (Cỡ lớn)", Price = 280000m, StockQuantity = 50, CategoryId = 3 },
                new Product { ProductId = 11, Barcode = "893000000011", ProductName = "Bộ dụng cụ nấu silicone chịu nhiệt (5 món)", Price = 195000m, StockQuantity = 70, CategoryId = 3 },
                new Product { ProductId = 12, Barcode = "893000000012", ProductName = "Hộp cơm giữ nhiệt văn phòng Lock&Lock 3 ngăn", Price = 390000m, StockQuantity = 45, CategoryId = 4 },
                new Product { ProductId = 13, Barcode = "893000000013", ProductName = "Set 4 hộp thủy tinh chịu nhiệt Borosilicate", Price = 320000m, StockQuantity = 60, CategoryId = 4 },
                new Product { ProductId = 14, Barcode = "893000000014", ProductName = "Bình giữ nhiệt thể thao Inox 304 (750ml)", Price = 220000m, StockQuantity = 80, CategoryId = 5 },
                new Product { ProductId = 15, Barcode = "893000000015", ProductName = "Bộ ấm chén trà gốm sứ Bát Tràng viền vàng", Price = 680000m, StockQuantity = 20, CategoryId = 5 },
                new Product { ProductId = 16, Barcode = "893000000016", ProductName = "Bếp từ đơn điều khiển cảm ứng Kangaroo", Price = 790000m, StockQuantity = 30, CategoryId = 2 },
                new Product { ProductId = 17, Barcode = "893000000017", ProductName = "Máy ép chậm nước trái cây hoa quả Lumias", Price = 1350000m, StockQuantity = 18, CategoryId = 8 },
                new Product { ProductId = 18, Barcode = "893000000018", ProductName = "Cân tiểu ly điện tử nhà bếp định lượng 5kg", Price = 150000m, StockQuantity = 65, CategoryId = 10 },
                new Product { ProductId = 19, Barcode = "893000000019", ProductName = "Kệ để dao thớt và gia vị Inox 304 (2 tầng)", Price = 380000m, StockQuantity = 40, CategoryId = 11 },
                new Product { ProductId = 20, Barcode = "893000000020", ProductName = "Giá úp bát đĩa có khay thoát nước thông minh", Price = 420000m, StockQuantity = 35, CategoryId = 11 }
            );

            // 4. DATA SEEDING: BẢNG CUSTOMERS (15 Khách hàng thân thiết Kitchen Express tại TP.HCM)
            modelBuilder.Entity<Customer>().HasData(
                new Customer { CustomerId = 1, CustomerName = "Nguyễn Văn An", PhoneNumber = "0901122334", Address = "123 Nguyễn Trãi, Quận 1, TPHCM", RewardPoints = 150, MembershipRank = "Vàng" },
                new Customer { CustomerId = 2, CustomerName = "Trần Thị Bình", PhoneNumber = "0918877665", Address = "45 Lê Lợi, Quận Bình Thạnh, TPHCM", RewardPoints = 50, MembershipRank = "Bạc" },
                new Customer { CustomerId = 3, CustomerName = "Lê Văn Cường", PhoneNumber = "0983344556", Address = "78 Cách Mạng Tháng 8, Quận 3, TPHCM", RewardPoints = 45, MembershipRank = "Chuẩn" },
                new Customer { CustomerId = 4, CustomerName = "Phạm Thị Dung", PhoneNumber = "0932233445", Address = "102 Quang Trung, Quận Gò Vấp, TPHCM", RewardPoints = 520, MembershipRank = "Bạch Kim" },
                new Customer { CustomerId = 5, CustomerName = "Hoàng Văn Em", PhoneNumber = "0977889900", Address = "15 Phan Xích Long, Quận Phú Nhuận, TPHCM", RewardPoints = 80, MembershipRank = "Chuẩn" },
                new Customer { CustomerId = 6, CustomerName = "Vũ Thị Phương", PhoneNumber = "0912345678", Address = "234 Hoàng Văn Thụ, Quận Tân Bình, TPHCM", RewardPoints = 210, MembershipRank = "Bạc" },
                new Customer { CustomerId = 7, CustomerName = "Đặng Văn Giang", PhoneNumber = "0988776655", Address = "312 Ba Tháng Hai, Quận 10, TPHCM", RewardPoints = 310, MembershipRank = "Vàng" },
                new Customer { CustomerId = 8, CustomerName = "Bùi Thị Hoa", PhoneNumber = "0905544332", Address = "56 Nguyễn Thị Thập, Quận 7, TPHCM", RewardPoints = 410, MembershipRank = "Vàng" },
                new Customer { CustomerId = 9, CustomerName = "Ngô Văn Hùng", PhoneNumber = "0966112233", Address = "79 Kinh Dương Vương, Quận Bình Tân, TPHCM", RewardPoints = 95, MembershipRank = "Chuẩn" },
                new Customer { CustomerId = 10, CustomerName = "Dương Thị Lan", PhoneNumber = "0944332211", Address = "18 Hoàng Diệu, Quận 4, TPHCM", RewardPoints = 650, MembershipRank = "Bạch Kim" },
                new Customer { CustomerId = 11, CustomerName = "Lý Văn Minh", PhoneNumber = "0922334455", Address = "405 Lê Văn Khương, Quận 12, TPHCM", RewardPoints = 30, MembershipRank = "Chuẩn" },
                new Customer { CustomerId = 12, CustomerName = "Đỗ Thị Nga", PhoneNumber = "0933445566", Address = "123 Long Thuận, TP. Thủ Đức, TPHCM", RewardPoints = 180, MembershipRank = "Bạc" },
                new Customer { CustomerId = 13, CustomerName = "Hồ Văn Quân", PhoneNumber = "0977112233", Address = "67 Lũy Bán Bích, Quận Tân Phú, TPHCM", RewardPoints = 290, MembershipRank = "Bạc" },
                new Customer { CustomerId = 14, CustomerName = "Phan Thị Sương", PhoneNumber = "0909988776", Address = "142 Phạm Thế Hiển, Quận 8, TPHCM", RewardPoints = 480, MembershipRank = "Vàng" },
                new Customer { CustomerId = 15, CustomerName = "Trịnh Văn Tùng (Khách vãng lai)", PhoneNumber = "0911223344", Address = "89 An Dương Vương, Quận 5, TPHCM", RewardPoints = 0, MembershipRank = "Chuẩn" }
            );

            // 5. DATA SEEDING: BẢNG ORDERS (15 Hóa đơn bán lẻ thiết bị gia dụng Kitchen Express)
            modelBuilder.Entity<Order>().HasData(
                new Order { OrderId = 1, OrderCode = "HD-KE20261001-001", CreatedDate = new DateTime(2026, 10, 1, 8, 30, 0), CustomerId = 1, CashierUsername = "cashier01", TotalItems = 2, FinalAmount = 3010000m, RewardPoints = 301 },
                new Order { OrderId = 2, OrderCode = "HD-KE20261001-002", CreatedDate = new DateTime(2026, 10, 1, 9, 15, 0), CustomerId = 2, CashierUsername = "cashier01", TotalItems = 2, FinalAmount = 1130000m, RewardPoints = 113 },
                new Order { OrderId = 3, OrderCode = "HD-KE20261001-003", CreatedDate = new DateTime(2026, 10, 1, 10, 0, 0), CustomerId = 3, CashierUsername = "cashier02", TotalItems = 3, FinalAmount = 915000m, RewardPoints = 91 },
                new Order { OrderId = 4, OrderCode = "HD-KE20261002-001", CreatedDate = new DateTime(2026, 10, 2, 8, 45, 0), CustomerId = 4, CashierUsername = "cashier02", TotalItems = 1, FinalAmount = 1850000m, RewardPoints = 185 },
                new Order { OrderId = 5, OrderCode = "HD-KE20261002-002", CreatedDate = new DateTime(2026, 10, 2, 11, 20, 0), CustomerId = 5, CashierUsername = "cashier03", TotalItems = 2, FinalAmount = 1730000m, RewardPoints = 173 },
                new Order { OrderId = 6, OrderCode = "HD-KE20261003-001", CreatedDate = new DateTime(2026, 10, 3, 14, 10, 0), CustomerId = 6, CashierUsername = "cashier03", TotalItems = 1, FinalAmount = 1790000m, RewardPoints = 179 },
                new Order { OrderId = 7, OrderCode = "HD-KE20261003-002", CreatedDate = new DateTime(2026, 10, 3, 16, 40, 0), CustomerId = 7, CashierUsername = "cashier04", TotalItems = 2, FinalAmount = 800000m, RewardPoints = 80 },
                new Order { OrderId = 8, OrderCode = "HD-KE20261004-001", CreatedDate = new DateTime(2026, 10, 4, 9, 05, 0), CustomerId = 8, CashierUsername = "cashier04", TotalItems = 1, FinalAmount = 2490000m, RewardPoints = 249 },
                new Order { OrderId = 9, OrderCode = "HD-KE20261004-002", CreatedDate = new DateTime(2026, 10, 4, 13, 30, 0), CustomerId = 9, CashierUsername = "cashier05", TotalItems = 2, FinalAmount = 875000m, RewardPoints = 87 },
                new Order { OrderId = 10, OrderCode = "HD-KE20261005-001", CreatedDate = new DateTime(2026, 10, 5, 10, 50, 0), CustomerId = 10, CashierUsername = "cashier05", TotalItems = 3, FinalAmount = 2170000m, RewardPoints = 217 },
                new Order { OrderId = 11, OrderCode = "HD-KE20261005-002", CreatedDate = new DateTime(2026, 10, 5, 17, 15, 0), CustomerId = 11, CashierUsername = "cashier06", TotalItems = 2, FinalAmount = 740000m, RewardPoints = 74 },
                new Order { OrderId = 12, OrderCode = "HD-KE20261006-001", CreatedDate = new DateTime(2026, 10, 6, 8, 20, 0), CustomerId = 12, CashierUsername = "cashier06", TotalItems = 1, FinalAmount = 1350000m, RewardPoints = 135 },
                new Order { OrderId = 13, OrderCode = "HD-KE20261006-002", CreatedDate = new DateTime(2026, 10, 6, 15, 00, 0), CustomerId = 13, CashierUsername = "cashier07", TotalItems = 2, FinalAmount = 900000m, RewardPoints = 90 },
                new Order { OrderId = 14, OrderCode = "HD-KE20261007-001", CreatedDate = new DateTime(2026, 10, 7, 12, 10, 0), CustomerId = 14, CashierUsername = "cashier07", TotalItems = 1, FinalAmount = 1250000m, RewardPoints = 125 },
                new Order { OrderId = 15, OrderCode = "HD-KE20261007-002", CreatedDate = new DateTime(2026, 10, 7, 18, 45, 0), CustomerId = 15, CashierUsername = "cashier01", TotalItems = 2, FinalAmount = 570000m, RewardPoints = 57 }
            );

            // 6. DATA SEEDING: BẢNG ORDERDETAILS (18 Dòng chi tiết đơn hàng đồ gia dụng)
            modelBuilder.Entity<OrderDetail>().HasData(
                // Đơn 1: 1 Nồi chiên Philips (2.49M), 1 Ấm siêu tốc Lock&Lock (520k) => 3.01M
                new OrderDetail { OrderDetailId = 1, OrderId = 1, ProductId = 1, Quantity = 1, UnitPrice = 2490000m, LineTotal = 2490000m },
                new OrderDetail { OrderDetailId = 2, OrderId = 1, ProductId = 5, Quantity = 1, UnitPrice = 520000m, LineTotal = 520000m },

                // Đơn 2: 1 Máy xay Sunhouse (650k), 1 Chảo Tefal 28cm (480k) => 1.13M
                new OrderDetail { OrderDetailId = 3, OrderId = 2, ProductId = 2, Quantity = 1, UnitPrice = 650000m, LineTotal = 650000m },
                new OrderDetail { OrderDetailId = 4, OrderId = 2, ProductId = 7, Quantity = 1, UnitPrice = 480000m, LineTotal = 480000m },

                // Đơn 3: 1 Bộ dao bếp (450k), 1 Thớt gỗ Teak (280k), 1 Dụng cụ silicon (195k) => 925k (khuyến mãi 915k)
                new OrderDetail { OrderDetailId = 5, OrderId = 3, ProductId = 9, Quantity = 1, UnitPrice = 450000m, LineTotal = 450000m },
                new OrderDetail { OrderDetailId = 6, OrderId = 3, ProductId = 10, Quantity = 1, UnitPrice = 280000m, LineTotal = 280000m },
                new OrderDetail { OrderDetailId = 7, OrderId = 3, ProductId = 11, Quantity = 1, UnitPrice = 185000m, LineTotal = 185000m },

                // Đơn 4: 1 Nồi cơm điện Panasonic 1.8L (1.85M)
                new OrderDetail { OrderDetailId = 8, OrderId = 4, ProductId = 3, Quantity = 1, UnitPrice = 1850000m, LineTotal = 1850000m },

                // Đơn 5: 1 Bộ nồi Elmich (1.25M), 1 Chảo Tefal (480k) => 1.73M
                new OrderDetail { OrderDetailId = 9, OrderId = 5, ProductId = 6, Quantity = 1, UnitPrice = 1250000m, LineTotal = 1250000m },
                new OrderDetail { OrderDetailId = 10, OrderId = 5, ProductId = 7, Quantity = 1, UnitPrice = 480000m, LineTotal = 480000m },

                // Đơn 6: 1 Lò vi sóng Sharp (1.79M)
                new OrderDetail { OrderDetailId = 11, OrderId = 6, ProductId = 4, Quantity = 1, UnitPrice = 1790000m, LineTotal = 1790000m },

                // Đơn 7: 1 Giá úp bát đĩa (420k), 1 Kệ dao thớt inox (380k) => 800k
                new OrderDetail { OrderDetailId = 12, OrderId = 7, ProductId = 20, Quantity = 1, UnitPrice = 420000m, LineTotal = 420000m },
                new OrderDetail { OrderDetailId = 13, OrderId = 7, ProductId = 19, Quantity = 1, UnitPrice = 380000m, LineTotal = 380000m },

                // Đơn 8: 1 Nồi chiên không dầu Philips (2.49M)
                new OrderDetail { OrderDetailId = 14, OrderId = 8, ProductId = 1, Quantity = 1, UnitPrice = 2490000m, LineTotal = 2490000m },

                // Đơn 9: 1 Bộ ấm chén Bát Tràng (680k), 1 Bộ dụng cụ silicon (195k) => 875k
                new OrderDetail { OrderDetailId = 15, OrderId = 9, ProductId = 15, Quantity = 1, UnitPrice = 680000m, LineTotal = 680000m },
                new OrderDetail { OrderDetailId = 16, OrderId = 9, ProductId = 11, Quantity = 1, UnitPrice = 195000m, LineTotal = 195000m },

                // Đơn 10: 1 Bếp từ Kangaroo (790k), 1 Máy ép chậm Lumias (1.35M), 1 Cân tiểu ly (150k) (khuyến mãi combo 2.17M)
                new OrderDetail { OrderDetailId = 17, OrderId = 10, ProductId = 16, Quantity = 1, UnitPrice = 790000m, LineTotal = 790000m },
                new OrderDetail { OrderDetailId = 18, OrderId = 10, ProductId = 17, Quantity = 1, UnitPrice = 1350000m, LineTotal = 1350000m }
            );
        }
    }
}
