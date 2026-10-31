using System;
using System.Text;
using Microsoft.Data.SqlClient;

namespace ThucHanhAdoNet
{
    internal class Program
    {
        private const string ConnString = "Server=localhost;Database=MiniSupermarketDb;Trusted_Connection=True;TrustServerCertificate=True;";

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            if (args.Length > 0)
            {
                switch (args[0].Trim())
                {
                    case "1": RunBaiTap1(); break;
                    case "2": RunBaiTap2(); break;
                    case "3": RunBaiTap3(); break;
                    case "4": RunBaiTapTiet4(); break;
                    default: Console.WriteLine("Tham số không hợp lệ (1-4)."); break;
                }
                return;
            }

            while (true)
            {
                try { Console.Clear(); } catch { }
                Console.WriteLine("===================================================================================");
                Console.WriteLine("   KITCHEN EXPRESS - HỆ THỐNG QUẢN LÝ THIẾT BỊ GIA DỤNG NHÀ BẾP (ADO.NET)          ");
                Console.WriteLine("===================================================================================");
                Console.WriteLine("1. Bài tập 1 (Tiết 1): Đếm tổng số mặt hàng thiết bị bếp bằng ExecuteScalar()");
                Console.WriteLine("2. Bài tập 2 (Tiết 1): Cập nhật tồn kho thiết bị an toàn bằng ExecuteNonQuery()");
                Console.WriteLine("3. Bài tập 3 (Tiết 1): Đọc danh mục sản phẩm nhà bếp bằng SqlDataReader");
                Console.WriteLine("4. Bài tập Tiết 4   : Thử nghiệm SqlTransaction Rollback khi thiếu hàng (ACID)");
                Console.WriteLine("0. Thoát chương trình");
                Console.WriteLine("===================================================================================");
                Console.Write("Vui lòng chọn bài tập thực hành (0 - 4): ");

                string choice = Console.ReadLine() ?? "";
                Console.WriteLine();

                switch (choice.Trim())
                {
                    case "1":
                        RunBaiTap1();
                        break;
                    case "2":
                        RunBaiTap2();
                        break;
                    case "3":
                        RunBaiTap3();
                        break;
                    case "4":
                        RunBaiTapTiet4();
                        break;
                    case "0":
                        Console.WriteLine("Tạm biệt!");
                        return;
                    default:
                        Console.WriteLine("Lựa chọn không hợp lệ!");
                        break;
                }

                Console.WriteLine("\nNhấn Enter để quay lại Menu chính...");
                Console.ReadLine();
            }
        }

        // =========================================================================
        // BÀI TẬP 1: Đếm tổng số mặt hàng bằng ExecuteScalar()
        // =========================================================================
        static void RunBaiTap1()
        {
            Console.WriteLine("=== BÀI TẬP 1: ĐẾM TỔNG SỐ THIẾT BỊ GIA DỤNG NHÀ BẾP (ExecuteScalar) ===");
            using (SqlConnection connection = new SqlConnection(ConnString))
            {
                try
                {
                    string query = "SELECT COUNT(*) FROM Products";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        Console.WriteLine("-> Đã kết nối thành công tới SQL Server Kitchen Express!");
                        object result = command.ExecuteScalar();
                        int totalCount = Convert.ToInt32(result);
                        Console.WriteLine($"-> Tổng số mặt hàng thiết bị bếp hiện có trong kho: {totalCount} sản phẩm.");
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"[LỖI SQL]: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[LỖI HỆ THỐNG]: {ex.Message}");
                }
            }
        }

        // =========================================================================
        // BÀI TẬP 2: Cập nhật tồn kho an toàn bằng ExecuteNonQuery() và SqlParameter
        // =========================================================================
        static void RunBaiTap2()
        {
            Console.WriteLine("=== BÀI TẬP 2: CẬP NHẬT TỒN KHO AN TOÀN VỚI SQLPARAMETER (ExecuteNonQuery) ===");
            int maSanPhamCanSua = 1; // Nồi chiên không dầu Philips 5L
            int soLuongTonKhoMoi = 35;

            using (SqlConnection connection = new SqlConnection(ConnString))
            {
                try
                {
                    string updateSql = @"UPDATE Products 
                                          SET StockQuantity = @Stock 
                                          WHERE ProductId = @Id";
                    using (SqlCommand command = new SqlCommand(updateSql, connection))
                    {
                        command.Parameters.AddWithValue("@Stock", soLuongTonKhoMoi);
                        command.Parameters.AddWithValue("@Id", maSanPhamCanSua);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine($"-> THÀNH CÔNG: Đã cập nhật tồn kho cho Sản phẩm ID = {maSanPhamCanSua} thành {soLuongTonKhoMoi} đơn vị.");
                            Console.WriteLine($"-> Số dòng dữ liệu bị ảnh hưởng: {rowsAffected} dòng.");
                        }
                        else
                        {
                            Console.WriteLine($"-> CẢNH BÁO: Không tìm thấy sản phẩm có ID = {maSanPhamCanSua} để cập nhật.");
                        }
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"[LỖI SQL]: {ex.Message}");
                }
            }
        }

        // =========================================================================
        // BÀI TẬP 3: Đọc danh sách sản phẩm bằng SqlDataReader
        // =========================================================================
        static void RunBaiTap3()
        {
            Console.WriteLine("=== BÀI TẬP 3: DUYỆT DANH SÁCH SẢN PHẨM NHÀ BẾP KITCHEN EXPRESS (ExecuteReader) ===");
            using (SqlConnection connection = new SqlConnection(ConnString))
            {
                try
                {
                    string query = @"SELECT ProductId, Barcode, ProductName, Price, StockQuantity 
                                      FROM Products 
                                      ORDER BY ProductId ASC";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Console.WriteLine("---------------------------------------------------------------------------------------------------------");
                            Console.WriteLine(string.Format("{0,-6} | {1,-14} | {2,-46} | {3,14} | {4,8}",
                                "MÃ SP", "MÃ VẠCH", "TÊN THIẾT BỊ / ĐỒ DÙNG BẾP", "ĐƠN GIÁ", "TỒN KHO"));
                            Console.WriteLine("---------------------------------------------------------------------------------------------------------");

                            int stt = 0;
                            while (reader.Read())
                            {
                                stt++;
                                int id = Convert.ToInt32(reader["ProductId"]);
                                string barcode = reader["Barcode"].ToString() ?? "";
                                string name = reader["ProductName"].ToString() ?? "";
                                decimal price = Convert.ToDecimal(reader["Price"]);
                                int stock = Convert.ToInt32(reader["StockQuantity"]);

                                Console.WriteLine(string.Format("{0,-6} | {1,-14} | {2,-46} | {3,14:N0} đ | {4,8}",
                                    id, barcode, name, price, stock));
                            }
                            Console.WriteLine("---------------------------------------------------------------------------------------------------------");
                            Console.WriteLine($"-> Tổng cộng đã đọc thành công: {stt} mặt hàng thiết bị nhà bếp Kitchen Express.");
                        }
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"[LỖI SQL]: {ex.Message}");
                }
            }
        }

        // =========================================================================
        // TIẾT 4: Kiểm chứng cơ chế Rollback của SqlTransaction (ACID)
        // =========================================================================
        static void RunBaiTapTiet4()
        {
            Console.WriteLine("=== THỬ NGHIỆM ĐIỀU KHIỂN SQLTRANSACTION & KIỂM CHỨNG ROLLBACK (KITCHEN EXPRESS) ===");
            using (SqlConnection connection = new SqlConnection(ConnString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                try
                {
                    Console.WriteLine("-> Bắt đầu phiên giao dịch (transaction)...");

                    // Bước 1: Trừ 2 đơn vị tồn kho của SP 1 (Nồi chiên không dầu Philips)
                    string sqlStep1 = "UPDATE Products SET StockQuantity = StockQuantity - @Qty WHERE ProductId = @Id";
                    using (SqlCommand cmd1 = new SqlCommand(sqlStep1, connection, transaction))
                    {
                        cmd1.Parameters.AddWithValue("@Qty", 2);
                        cmd1.Parameters.AddWithValue("@Id", 1);
                        int r1 = cmd1.ExecuteNonQuery();
                        Console.WriteLine($"-> Bước 1: Đã trừ tồn kho Nồi chiên không dầu Philips (Số dòng tác động: {r1}).");
                    }

                    // Bước 2: Giả lập kiểm tra tồn kho mặt hàng thứ 2 bị thiếu hàng -> Cố tình ném Exception
                    Console.WriteLine("-> Bước 2: Chuẩn bị kiểm tra tồn kho mặt hàng thứ 2...");
                    bool isInventoryShortage = true;
                    if (isInventoryShortage)
                    {
                        throw new InvalidOperationException("Mặt hàng thứ hai (Lò vi sóng Sharp) đã hết tồn kho thực tế!");
                    }

                    transaction.Commit();
                    Console.WriteLine("-> GIAO DỊCH COMMIT THÀNH CÔNG!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n[PHÁT SINH LỖI]: {ex.Message}");
                    transaction.Rollback();
                    Console.WriteLine("-> [HỆ THỐNG]: Đã kích hoạt transaction.Rollback() an toàn!");
                    Console.WriteLine("-> Toàn bộ thay đổi của Bước 1 đã được thu hồi, tồn kho Nồi chiên không dầu Philips được giữ nguyên!");
                }
            }
        }
    }
}
