🛒 HỆ THỐNG QUẢN LÝ SIÊU THỊ MINI (MINISUPERMARKET SYSTEM)

Môn học: Lập trình Ứng dụng .NET Core (Mã môn: 229162) Phiên bản: Hoàn thiện đồ án (tổng hợp Buổi 1 → Buổi 5)

Hệ thống gồm một Web API (ASP.NET Core) và một ứng dụng WinForms đóng vai trò máy trạm POS tại quầy. Dữ liệu mẫu mô phỏng cửa hàng đồ gia dụng nhà bếp Kitchen Express.

🎯 1. Chức năng chính
Quản lý nhóm hàng, sản phẩm, khách hàng, tài khoản người dùng (thêm, sửa, xóa, tìm kiếm, phân trang).
Đăng nhập bằng JWT Token, có đăng nhập/khóa/đặt lại mật khẩu cho tài khoản.
Bán hàng POS: quét mã vạch, lập hóa đơn, trừ tồn kho, tích điểm thưởng khách hàng. Toàn bộ bọc trong một SqlTransaction, tự Rollback khi lỗi hoặc hết hàng.
Hủy hóa đơn: hoàn trả tồn kho và khấu trừ điểm thưởng.
Báo cáo: doanh thu theo thu ngân, doanh thu theo khoảng ngày, top 5 sản phẩm bán chạy, báo cáo nhanh theo ngày.
🏗️ 2. Kiến trúc hệ thống (Client - Server)
Thành phần	Mô tả
MiniSupermarket.API (Backend)	ASP.NET Core Web API (.NET 8). Xử lý nghiệp vụ, truy cập SQL Server bằng EF Core và ADO.NET, cấp JWT, cung cấp RESTful API và Swagger UI.
MiniSupermarket.WinForms (Client)	Windows Forms (.NET Framework 4.7.2). Gọi API bằng HttpClient, hiển thị lên DataGridView.
SQL Server	Cơ sở dữ liệu MiniSupermarketDb, tạo bằng EF Core Migrations.
🛠️ 3. Công nghệ sử dụng
Backend: C#, .NET 8, ASP.NET Core Web API, Entity Framework Core 8 (SQL Server), ADO.NET (Microsoft.Data.SqlClient), JWT Bearer (System.IdentityModel.Tokens.Jwt), LINQ.
Frontend: Windows Forms (.NET Framework 4.7.2), HttpClient.
Cơ sở dữ liệu: Microsoft SQL Server (Stored Procedure, Transaction).
Kiểm thử: Swagger UI (Swashbuckle), script Python test_suite.py.
📂 4. Cấu trúc Solution
MiniSupermarket.API/                 (thư mục gốc repo)
│
├── MiniSupermarket.API/             # Dự án Web API (Backend)
│   ├── Controllers/                 # Auth, Categories, Products, Customers, Orders,
│   │                                # Checkout, AdoProduct, Reports, Users
│   ├── Models/                      # User, Category, Product, Customer, Order, OrderDetail
│   │                                # + DTO (PagedResponseDto, CheckoutDtos, CashierRevenueDto)
│   ├── Data/SupermarketDbContext.cs # DbContext, khóa ngoại, dữ liệu mẫu (Seed Data)
│   ├── Migrations/                  # EF Core Migrations
│   ├── appsettings.json             # Chuỗi kết nối + khóa JWT
│   └── Program.cs                   # Đăng ký dịch vụ, JWT, Swagger, Middleware
│
├── MiniSupermarket.WinForms/        # Ứng dụng WinForms (Client)
│   ├── FormLogin / FormMainShell    # Đăng nhập và khung điều hướng chính
│   ├── FormPOS                      # Bán hàng
│   ├── FormCategoryManagement       # Quản lý nhóm hàng
│   ├── FormProductManagement        # Quản lý sản phẩm
│   ├── FormCustomerManagement       # Quản lý khách hàng
│   ├── FormUserManagement           # Quản lý tài khoản
│   ├── FormQuickReport              # Báo cáo nhanh
│   └── Services/ApiClientService.cs # Gọi API
│
├── ThucHanhAdoNet/                  # Ứng dụng Console thực hành ADO.NET
├── ReportGenerator/                 # Công cụ hỗ trợ tạo file báo cáo Word
├── test_suite.py, test_results.json # Kịch bản kiểm thử API và kết quả
├── BaoCao_KitchenExpress.docx       # Báo cáo đồ án
└── SQLQuery2.sql
🗄️ 5. Cơ sở dữ liệu

6 bảng: Users, Categories, Products, Customers, Orders, OrderDetails.

Quan hệ 1-N: Category – Product, Order – OrderDetail, Customer – Order.
Ràng buộc dữ liệu bằng Data Annotations ([Required], [StringLength], [Range]).
Dữ liệu mẫu (Seed Data) nạp sẵn khi tạo CSDL: 15 tài khoản, 15 nhóm hàng, 20 sản phẩm, 15 khách hàng.
Migration: InitFullDatabaseWithSeedData.
🔌 6. Danh sách API chính

Tất cả đường dẫn bắt đầu bằng /api.

Nhóm	Phương thức và đường dẫn	Mô tả
Auth	POST /auth/login	Đăng nhập, trả về JWT Token
Categories	GET /categories, GET /categories/{id}, GET /categories/search	Xem, tìm kiếm nhóm hàng
	POST /categories, PUT /categories/{id}, DELETE /categories/{id}	Thêm, sửa, xóa (yêu cầu quyền Admin)
Products	GET /products, /products/paged, /products/with-category, /products/{id}, /products/barcode/{barcode}	Danh sách, phân trang (Skip/Take), kèm tên nhóm (Include), tra theo mã vạch
	POST /products, PUT /products/{id}, DELETE /products/{id}	Thêm, sửa, xóa sản phẩm
Customers	GET /customers, /customers/paged, /customers/{id}, /customers/search	Danh sách, phân trang, tìm kiếm
	POST /customers, PUT /customers/{id}, DELETE /customers/{id}	Thêm, sửa, xóa khách hàng
Orders	GET /orders, GET /orders/{id}, POST /orders/checkout	Xem hóa đơn, thanh toán bằng EF Core
Checkout (ADO.NET)	POST /checkout	Thanh toán POS trong SqlTransaction, tự Rollback khi lỗi
	DELETE /checkout/cancel/{orderId}	Hủy hóa đơn, hoàn kho, trừ điểm
AdoProducts (ADO.NET)	GET /ado-products/barcode/{barcode}	Tra mã vạch bằng SqlCommand và SqlDataReader
	GET /ado-products/disconnected-cache, /filter-stock, /count-by-category	DataTable/SqlDataAdapter, lọc tồn kho thấp, đếm theo nhóm
Reports	GET /reports/revenue-by-cashier	Gọi Stored Procedure sp_GetRevenueByCashier
	GET /reports/revenue-by-date?from=&to=	Gọi Stored Procedure sp_GetRevenueByDateRange
	GET /reports/top5-products	Gọi Stored Procedure sp_GetTop5SellingProducts
	GET /reports/daily	Báo cáo nhanh theo ngày
Users	GET /users, POST /users, PUT /users/{id}/reset-password, PUT /users/{id}/toggle-lock	Quản lý tài khoản (yêu cầu quyền Admin)
Xác thực và phân quyền
Đăng nhập tại POST /api/auth/login, API trả về JWT Token (hạn 2 giờ, ký HS256).
Gửi token trong header Authorization: Bearer <token>.
Hiện đã gắn [Authorize(Roles = "Admin")] cho UsersController và các thao tác thêm, sửa, xóa của CategoriesController.
Có 3 vai trò: Admin, Cashier (thu ngân), Warehouse (thủ kho).
📌 7. Nội dung theo từng buổi
Buổi	Nhánh Git	Nội dung
1	buoi-1	Web API quản lý nhóm hàng, kết nối WinForms Client (CRUD)
2	buoi-2	Đăng nhập JWT, [Authorize], phân quyền theo vai trò
3	buoi-3	EF Core + SQL Server, Migrations, Seed Data
4	buoi-4	LINQ (tìm kiếm, lọc, phân trang), DTO, Include, AsNoTracking, Products/Customers/Orders/Users và các form WinForms
5	buoi-5	ADO.NET (SqlDataReader, chống SQL Injection bằng tham số), Stored Procedure, SqlTransaction + Rollback, báo cáo
Kết thúc	master, ket-thuc	Bản hoàn thiện đồ án
🚀 8. Hướng dẫn Chạy và Kiểm thử
Yêu cầu
Visual Studio 2022, .NET 8 SDK, .NET Framework 4.7.2 Developer Pack.
Microsoft SQL Server (mặc định localhost, đăng nhập Windows Authentication).
Bước 1: Tạo cơ sở dữ liệu
Kiểm tra chuỗi kết nối trong MiniSupermarket.API/appsettings.json (mục ConnectionStrings:DefaultConnection), sửa nếu SQL Server của bạn khác.
Mở Package Manager Console, chọn project MiniSupermarket.API và chạy:
   Update-Database

(hoặc dotnet ef database update trong thư mục MiniSupermarket.API). Lệnh này tạo CSDL MiniSupermarketDb cùng dữ liệu mẫu. 3. Tạo 3 Stored Procedure trong SQL Server để các API báo cáo hoạt động: sp_GetRevenueByCashier, sp_GetRevenueByDateRange (tham số @FromDate, @ToDate), sp_GetTop5SellingProducts.

Bước 2: Chạy Backend (Web API)
Chọn project MiniSupermarket.API làm Startup Project và nhấn F5 (profile https, địa chỉ https://localhost:7054).
Mở Swagger UI tại https://localhost:7054/swagger.
Gọi POST /api/auth/login để lấy token, bấm nút Authorize (ổ khóa), dán token vào rồi bấm Execute thử các API.

Tài khoản mẫu:

Tài khoản	Mật khẩu	Vai trò
admin	admin@123	Admin
cashier01	cashier@123	Cashier
warehouse01	wh@123	Warehouse
staff_lock	lock@123	Tài khoản bị khóa (để thử trường hợp đăng nhập thất bại)
Bước 3: Chạy Frontend (WinForms Client)
Đảm bảo địa chỉ https://localhost:7054/api/ trong ApiClient.cs và Services/ApiClientService.cs khớp với cổng của Web API đang chạy.
Chạy API trước, sau đó chuột phải project MiniSupermarket.WinForms chọn Debug → Start new instance (hoặc đặt cả hai project cùng khởi động bằng Multiple startup projects).
Đăng nhập, dùng các chức năng: bán hàng POS, quản lý nhóm hàng, sản phẩm, khách hàng, tài khoản, xem báo cáo.
Bước 4: Kiểm thử tự động và thực hành ADO.NET
Chạy API bằng profile http (http://localhost:5010), rồi chạy python test_suite.py. Kết quả lưu ở test_results.json.
Ứng dụng Console ThucHanhAdoNet: dotnet run --project ThucHanhAdoNet -- 1 (thay 1 bằng 2 hoặc 3 để chạy bài tập tương ứng).

Lưu ý: Mật khẩu mẫu hiện lưu dạng văn bản thô trong cột PasswordHash nhằm phục vụ học tập, chưa được băm.

👨‍💻 9. Tác giả
Họ tên sinh viên: Đào Xuân Sơn
Mã sinh viên: 2124110105
Lớp học phần: CCQ2411C
