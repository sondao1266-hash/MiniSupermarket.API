using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MiniSupermarket.API.Migrations
{
    /// <inheritdoc />
    public partial class InitFullDatabaseWithSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RewardPoints = table.Column<int>(type: "int", nullable: false),
                    MembershipRank = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.CustomerId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Barcode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StockQuantity = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    CashierUsername = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TotalItems = table.Column<int>(type: "int", nullable: false),
                    FinalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RewardPoints = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_Orders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderDetails",
                columns: table => new
                {
                    OrderDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LineTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetails", x => x.OrderDetailId);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "CategoryName", "Description" },
                values: new object[,]
                {
                    { 1, "Nồi & Chảo chống dính", "Chảo từ, chảo sâu lòng, nồi inox 3 đáy, 5 đáy" },
                    { 2, "Thiết bị Điện nhà bếp", "Bếp từ, nồi cơm điện cao tần, ấm đun giữ nhiệt" },
                    { 3, "Dụng cụ Cắt thái & Sơ chế", "Bộ dao thép không gỉ, thớt gỗ kháng khuẩn, kéo bếp" },
                    { 4, "Hộp đựng & Bảo quản thực phẩm", "Hộp thủy tinh chịu nhiệt, hộp cơm giữ nhiệt văn phòng" },
                    { 5, "Dụng cụ Pha chế & Bàn ăn", "Bình giữ nhiệt inox 304, bộ ấm chén gốm sứ Bát Tràng" },
                    { 6, "Lò vi sóng & Thiết bị nướng", "Lò vi sóng điện tử, lò nướng thùng, vỉ nướng chống dính" },
                    { 7, "Nồi chiên & Nồi áp suất", "Nồi chiên không dầu công nghệ Rapid Air, nồi áp suất điện" },
                    { 8, "Máy xay & Máy ép hoa quả", "Máy xay sinh tố đa năng, máy ép chậm nước trái cây" },
                    { 9, "Thiết bị Pha trà & Cà phê", "Bình đun siêu tốc thông minh, phin pha, máy pha cà phê mini" },
                    { 10, "Dụng cụ Làm bánh & Định lượng", "Cân tiểu ly điện tử, khuôn bánh silicon, cây đánh trứng" },
                    { 11, "Kệ giá & Khay để đồ nhà bếp", "Kệ để dao thớt inox 304, giá úp bát đĩa thông minh" },
                    { 12, "Dụng cụ Vệ sinh nhà bếp", "Cọ xoong nồi đa năng, giá treo khăn lau, găng tay silicon" },
                    { 13, "Đồ sành sứ & Thủy tinh cao cấp", "Bát đĩa sứ xương, tô canh thủy tinh cường lực, ly thủy tinh" },
                    { 14, "Đũa, Muỗng & Dao muỗng nĩa", "Bộ muỗng nĩa inox mạ vàng, đũa gỗ bu-mê, muỗng canh silicon" },
                    { 15, "Vật tư & Phụ kiện Kitchen Express", "Giấy nến nồi chiên, màng bọc thực phẩm, túi zip đóng kín" }
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "CustomerId", "Address", "CustomerName", "MembershipRank", "PhoneNumber", "RewardPoints" },
                values: new object[,]
                {
                    { 1, "123 Nguyễn Trãi, Quận 1, TPHCM", "Nguyễn Văn An", "Vàng", "0901122334", 150 },
                    { 2, "45 Lê Lợi, Quận Bình Thạnh, TPHCM", "Trần Thị Bình", "Bạc", "0918877665", 50 },
                    { 3, "78 Cách Mạng Tháng 8, Quận 3, TPHCM", "Lê Văn Cường", "Chuẩn", "0983344556", 45 },
                    { 4, "102 Quang Trung, Quận Gò Vấp, TPHCM", "Phạm Thị Dung", "Bạch Kim", "0932233445", 520 },
                    { 5, "15 Phan Xích Long, Quận Phú Nhuận, TPHCM", "Hoàng Văn Em", "Chuẩn", "0977889900", 80 },
                    { 6, "234 Hoàng Văn Thụ, Quận Tân Bình, TPHCM", "Vũ Thị Phương", "Bạc", "0912345678", 210 },
                    { 7, "312 Ba Tháng Hai, Quận 10, TPHCM", "Đặng Văn Giang", "Vàng", "0988776655", 310 },
                    { 8, "56 Nguyễn Thị Thập, Quận 7, TPHCM", "Bùi Thị Hoa", "Vàng", "0905544332", 410 },
                    { 9, "79 Kinh Dương Vương, Quận Bình Tân, TPHCM", "Ngô Văn Hùng", "Chuẩn", "0966112233", 95 },
                    { 10, "18 Hoàng Diệu, Quận 4, TPHCM", "Dương Thị Lan", "Bạch Kim", "0944332211", 650 },
                    { 11, "405 Lê Văn Khương, Quận 12, TPHCM", "Lý Văn Minh", "Chuẩn", "0922334455", 30 },
                    { 12, "123 Long Thuận, TP. Thủ Đức, TPHCM", "Đỗ Thị Nga", "Bạc", "0933445566", 180 },
                    { 13, "67 Lũy Bán Bích, Quận Tân Phú, TPHCM", "Hồ Văn Quân", "Bạc", "0977112233", 290 },
                    { 14, "142 Phạm Thế Hiển, Quận 8, TPHCM", "Phan Thị Sương", "Vàng", "0909988776", 480 },
                    { 15, "89 An Dương Vương, Quận 5, TPHCM", "Trịnh Văn Tùng (Khách vãng lai)", "Chuẩn", "0911223344", 0 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "FullName", "IsActive", "PasswordHash", "Role", "Username" },
                values: new object[,]
                {
                    { 1, "Nguyễn Quản Trị - Kitchen Express", true, "admin@123", "Admin", "admin" },
                    { 2, "Trần Cửa Hàng Trưởng", true, "manager@123", "Admin", "manager01" },
                    { 3, "Lê Thu Ngân Quầy 1", true, "cashier@123", "Cashier", "cashier01" },
                    { 4, "Phạm Thu Ngân Quầy 2", true, "cashier@123", "Cashier", "cashier02" },
                    { 5, "Hoàng Thu Ngân Quầy 3", true, "cashier@123", "Cashier", "cashier03" },
                    { 6, "Vũ Thu Ngân Quầy 4", true, "cashier@123", "Cashier", "cashier04" },
                    { 7, "Đỗ Bán Hàng Thiết Bị", true, "cashier@123", "Cashier", "cashier05" },
                    { 8, "Ngô Tư Vấn Gia Dụng", true, "cashier@123", "Cashier", "cashier06" },
                    { 9, "Bùi Bán Lẻ Ca Tối", true, "cashier@123", "Cashier", "cashier07" },
                    { 10, "Đặng Quản Lý Kho Bếp", true, "wh@123", "Warehouse", "warehouse01" },
                    { 11, "Trương Thủ Kho Thiết Bị", true, "wh@123", "Warehouse", "warehouse02" },
                    { 12, "Lý Kiểm Kê Hàng Nhập", true, "wh@123", "Warehouse", "warehouse03" },
                    { 13, "Mai Bán Hàng Trực Tuyến", true, "cashier@123", "Cashier", "cashier08" },
                    { 14, "Dương Thu Ngân Cuối Tuần", true, "cashier@123", "Cashier", "cashier09" },
                    { 15, "Hồ Nhân Viên Nghỉ Việc", false, "lock@123", "Cashier", "staff_lock" }
                });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "OrderId", "CashierUsername", "CreatedDate", "CustomerId", "FinalAmount", "OrderCode", "RewardPoints", "TotalItems" },
                values: new object[,]
                {
                    { 1, "cashier01", new DateTime(2026, 10, 1, 8, 30, 0, 0, DateTimeKind.Unspecified), 1, 3010000m, "HD-KE20261001-001", 301, 2 },
                    { 2, "cashier01", new DateTime(2026, 10, 1, 9, 15, 0, 0, DateTimeKind.Unspecified), 2, 1130000m, "HD-KE20261001-002", 113, 2 },
                    { 3, "cashier02", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), 3, 915000m, "HD-KE20261001-003", 91, 3 },
                    { 4, "cashier02", new DateTime(2026, 10, 2, 8, 45, 0, 0, DateTimeKind.Unspecified), 4, 1850000m, "HD-KE20261002-001", 185, 1 },
                    { 5, "cashier03", new DateTime(2026, 10, 2, 11, 20, 0, 0, DateTimeKind.Unspecified), 5, 1730000m, "HD-KE20261002-002", 173, 2 },
                    { 6, "cashier03", new DateTime(2026, 10, 3, 14, 10, 0, 0, DateTimeKind.Unspecified), 6, 1790000m, "HD-KE20261003-001", 179, 1 },
                    { 7, "cashier04", new DateTime(2026, 10, 3, 16, 40, 0, 0, DateTimeKind.Unspecified), 7, 800000m, "HD-KE20261003-002", 80, 2 },
                    { 8, "cashier04", new DateTime(2026, 10, 4, 9, 5, 0, 0, DateTimeKind.Unspecified), 8, 2490000m, "HD-KE20261004-001", 249, 1 },
                    { 9, "cashier05", new DateTime(2026, 10, 4, 13, 30, 0, 0, DateTimeKind.Unspecified), 9, 875000m, "HD-KE20261004-002", 87, 2 },
                    { 10, "cashier05", new DateTime(2026, 10, 5, 10, 50, 0, 0, DateTimeKind.Unspecified), 10, 2170000m, "HD-KE20261005-001", 217, 3 },
                    { 11, "cashier06", new DateTime(2026, 10, 5, 17, 15, 0, 0, DateTimeKind.Unspecified), 11, 740000m, "HD-KE20261005-002", 74, 2 },
                    { 12, "cashier06", new DateTime(2026, 10, 6, 8, 20, 0, 0, DateTimeKind.Unspecified), 12, 1350000m, "HD-KE20261006-001", 135, 1 },
                    { 13, "cashier07", new DateTime(2026, 10, 6, 15, 0, 0, 0, DateTimeKind.Unspecified), 13, 900000m, "HD-KE20261006-002", 90, 2 },
                    { 14, "cashier07", new DateTime(2026, 10, 7, 12, 10, 0, 0, DateTimeKind.Unspecified), 14, 1250000m, "HD-KE20261007-001", 125, 1 },
                    { 15, "cashier01", new DateTime(2026, 10, 7, 18, 45, 0, 0, DateTimeKind.Unspecified), 15, 570000m, "HD-KE20261007-002", 57, 2 }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "Barcode", "CategoryId", "Price", "ProductName", "StockQuantity" },
                values: new object[,]
                {
                    { 1, "893000000001", 7, 2490000m, "Nồi chiên không dầu Philips 5L", 25 },
                    { 2, "893000000002", 8, 650000m, "Máy xay sinh tố đa năng Sunhouse", 40 },
                    { 3, "893000000003", 2, 1850000m, "Nồi cơm điện cao tần Panasonic 1.8L", 20 },
                    { 4, "893000000004", 6, 1790000m, "Lò vi sóng điện tử Sharp 20L", 15 },
                    { 5, "893000000005", 9, 520000m, "Ấm siêu tốc giữ nhiệt Lock&Lock 1.7L", 60 },
                    { 6, "893000000006", 1, 1250000m, "Bộ nồi inox 5 đáy Elmich 3 chiếc", 30 },
                    { 7, "893000000007", 1, 480000m, "Chảo chống dính sâu lòng Tefal 28cm", 55 },
                    { 8, "893000000008", 7, 890000m, "Nồi áp suất điện đa năng Sunhouse 5L", 25 },
                    { 9, "893000000009", 3, 450000m, "Bộ dao bếp cao cấp 6 món thép không gỉ", 35 },
                    { 10, "893000000010", 3, 280000m, "Thớt gỗ Teak kháng khuẩn cao cấp (Cỡ lớn)", 50 },
                    { 11, "893000000011", 3, 195000m, "Bộ dụng cụ nấu silicone chịu nhiệt (5 món)", 70 },
                    { 12, "893000000012", 4, 390000m, "Hộp cơm giữ nhiệt văn phòng Lock&Lock 3 ngăn", 45 },
                    { 13, "893000000013", 4, 320000m, "Set 4 hộp thủy tinh chịu nhiệt Borosilicate", 60 },
                    { 14, "893000000014", 5, 220000m, "Bình giữ nhiệt thể thao Inox 304 (750ml)", 80 },
                    { 15, "893000000015", 5, 680000m, "Bộ ấm chén trà gốm sứ Bát Tràng viền vàng", 20 },
                    { 16, "893000000016", 2, 790000m, "Bếp từ đơn điều khiển cảm ứng Kangaroo", 30 },
                    { 17, "893000000017", 8, 1350000m, "Máy ép chậm nước trái cây hoa quả Lumias", 18 },
                    { 18, "893000000018", 10, 150000m, "Cân tiểu ly điện tử nhà bếp định lượng 5kg", 65 },
                    { 19, "893000000019", 11, 380000m, "Kệ để dao thớt và gia vị Inox 304 (2 tầng)", 40 },
                    { 20, "893000000020", 11, 420000m, "Giá úp bát đĩa có khay thoát nước thông minh", 35 }
                });

            migrationBuilder.InsertData(
                table: "OrderDetails",
                columns: new[] { "OrderDetailId", "LineTotal", "OrderId", "ProductId", "Quantity", "UnitPrice" },
                values: new object[,]
                {
                    { 1, 2490000m, 1, 1, 1, 2490000m },
                    { 2, 520000m, 1, 5, 1, 520000m },
                    { 3, 650000m, 2, 2, 1, 650000m },
                    { 4, 480000m, 2, 7, 1, 480000m },
                    { 5, 450000m, 3, 9, 1, 450000m },
                    { 6, 280000m, 3, 10, 1, 280000m },
                    { 7, 185000m, 3, 11, 1, 185000m },
                    { 8, 1850000m, 4, 3, 1, 1850000m },
                    { 9, 1250000m, 5, 6, 1, 1250000m },
                    { 10, 480000m, 5, 7, 1, 480000m },
                    { 11, 1790000m, 6, 4, 1, 1790000m },
                    { 12, 420000m, 7, 20, 1, 420000m },
                    { 13, 380000m, 7, 19, 1, 380000m },
                    { 14, 2490000m, 8, 1, 1, 2490000m },
                    { 15, 680000m, 9, 15, 1, 680000m },
                    { 16, 195000m, 9, 11, 1, 195000m },
                    { 17, 790000m, 10, 16, 1, 790000m },
                    { 18, 1350000m, 10, 17, 1, 1350000m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_CategoryName",
                table: "Categories",
                column: "CategoryName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_PhoneNumber",
                table: "Customers",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OrderId",
                table: "OrderDetails",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ProductId",
                table: "OrderDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderCode",
                table: "Orders",
                column: "OrderCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_Barcode",
                table: "Products",
                column: "Barcode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderDetails");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
