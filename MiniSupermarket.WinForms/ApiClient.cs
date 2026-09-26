using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace MiniSupermarket.WinForms
{
    public static class ApiClient
    {
        private static readonly HttpClient _client = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7054/api/") // Đảm bảo đúng Port của Backend API nhé
        };

        public static HttpClient GetClient()
        {
            // Xóa sạch các authorization cũ trước đó để tránh trùng lặp
            _client.DefaultRequestHeaders.Authorization = null;

            // Nếu SessionManager đã có Token (đã đăng nhập), tự động gắn vào Header
            if (!string.IsNullOrEmpty(SessionManager.JwtToken))
            {
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", SessionManager.JwtToken);
            }

            return _client;
        }
    }
}