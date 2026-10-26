/*
 * Họ tên sinh viên: Nguyen Van Teo
 * Mã sinh viên: 123456789
 * Mô tả: Lớp tĩnh quản lý HttpClient kết nối Web API, tự động gắn JWT Bearer Token
 */
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace MiniSupermarket.WinForms.Services
{
    public static class ApiClientService
    {
        // Khởi tạo một phiên bản HttpClient dùng chung cho toàn ứng dụng
        public static readonly HttpClient Client = new HttpClient
        {
            // Đảm bảo số cổng Port trùng khớp với Web API Backend
            BaseAddress = new Uri("https://localhost:7054/api/")
        };

        // Phương thức cập nhật Bearer Token sau khi đăng nhập thành công
        public static void AttachToken(string token)
        {
            Client.DefaultRequestHeaders.Authorization = null; // Xóa token cũ
            if (!string.IsNullOrEmpty(token))
            {
                Client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        // Hàm gọi API đăng nhập lấy Token
        public static async Task<bool> LoginAsync(string username, string password)
        {
            var loginObj = new { Username = username, Password = password };
            var response = await Client.PostAsJsonAsync("auth/login", loginObj);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                using (var doc = JsonDocument.Parse(jsonString))
                {
                    string token = doc.RootElement.GetProperty("token").GetString() ?? string.Empty;
                    string role = doc.RootElement.GetProperty("role").GetString() ?? string.Empty;
                    SessionManager.JwtToken = token;
                    SessionManager.CurrentRole = role;
                    SessionManager.CurrentUsername = username;
                    if (doc.RootElement.TryGetProperty("fullName", out var fn))
                    {
                        SessionManager.FullName = fn.GetString() ?? username;
                    }
                    AttachToken(token);
                }
                return true;
            }
            return false;
        }

        // Hàm gọi API lấy dữ liệu có gắn kèm Bearer Token bảo mật
        public static async Task<string> GetDataWithTokenAsync(string endpoint)
        {
            AttachToken(SessionManager.JwtToken);
            var response = await Client.GetAsync(endpoint);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new Exception("Phiên làm việc hết hạn hoặc chưa đăng nhập!");
            }
            throw new Exception("Lỗi khi gọi dữ liệu từ Server.");
        }
    }
}
