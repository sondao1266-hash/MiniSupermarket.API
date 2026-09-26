using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniSupermarket.WinForms
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Chỉ khởi chạy FormLogin; FormCategoryManagement chỉ được mở
            // từ bên trong FormLogin.cs SAU KHI đăng nhập thành công.
            Application.Run(new FormLogin());
        }
    }
}