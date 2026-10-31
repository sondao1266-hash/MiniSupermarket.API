using System;

namespace MiniSupermarket.WinForms
{
    public static class SessionManager
    {
        public static string JwtToken { get; set; } = string.Empty;
        public static string CurrentRole { get; set; } = string.Empty;
        public static string CurrentUsername { get; set; } = string.Empty;
        public static string FullName { get; set; } = string.Empty;

        public static void Clear()
        {
            JwtToken = string.Empty;
            CurrentRole = string.Empty;
            CurrentUsername = string.Empty;
            FullName = string.Empty;
        }
    }
}
