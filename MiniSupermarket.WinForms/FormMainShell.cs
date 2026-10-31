using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MiniSupermarket.WinForms
{
    public partial class FormMainShell : Form
    {
        // ===== Các control (tạo bằng code) =====
        private Panel panelSidebar, panelTopHeader, panelMainContent, panelUserFooter;
        private Label lblTitle, lblUserInfo, lblLogo;
        private Button btnPOS, btnCategory, btnProduct, btnCustomer, btnReports, btnUserManage, btnLogout;

        private static readonly Color SidebarColor = Color.FromArgb(24, 30, 48);
        private static readonly Color ActiveColor = Color.FromArgb(41, 100, 180);

        private Form _activeForm = null;

        // Bảng quyền: vai trò -> các màn hình được phép
        private static readonly Dictionary<string, HashSet<string>> RoleScreens =
            new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["Admin"] = new HashSet<string> { "POS", "Category", "Product", "Customer", "Reports", "Users" },
                ["Cashier"] = new HashSet<string> { "POS", "Customer", "Reports" },
                ["Warehouse"] = new HashSet<string> { "Product", "Category" },
            };

        private static bool Can(string screen)
        {
            HashSet<string> set;
            return RoleScreens.TryGetValue(SessionManager.CurrentRole, out set) && set.Contains(screen);
        }

        public FormMainShell()
        {
            BuildUI();
            Load += FormMainShell_Load;
        }

        // ================= DỰNG GIAO DIỆN BẰNG CODE =================
        private Button MakeNavButton(string name, string text, EventHandler onClick)
        {
            var b = new Button
            {
                Name = name,
                Text = text,
                Dock = DockStyle.Top,
                Height = 45,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = SidebarColor,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(15, 0, 0, 0),
                Font = new Font("Segoe UI Emoji", 10f),
                Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderSize = 0;
            b.Click += onClick;
            return b;
        }

        private void BuildUI()
        {
            SuspendLayout();

            Text = "Kitchen Express - Hệ thống Quản lý Bán lẻ & Tồn kho Gia dụng Nhà bếp";
            ClientSize = new Size(1280, 720);
            StartPosition = FormStartPosition.CenterScreen;

            // --- Vùng nội dung chính ---
            panelMainContent = new Panel
            {
                Name = "panelMainContent",
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(244, 245, 247)
            };

            // --- Thanh tiêu đề trên ---
            panelTopHeader = new Panel
            {
                Name = "panelTopHeader",
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.White
            };
            lblUserInfo = new Label
            {
                Name = "lblUserInfo",
                Text = "Xin chào: ...",
                Dock = DockStyle.Right,
                Width = 520,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 10f),
                Padding = new Padding(0, 0, 15, 0)
            };
            lblTitle = new Label
            {
                Name = "lblTitle",
                UseMnemonic = false,
                Text = "BÀN LÀM VIỆC HỆ THỐNG",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                Padding = new Padding(15, 0, 0, 0)
            };
            panelTopHeader.Controls.Add(lblTitle);
            panelTopHeader.Controls.Add(lblUserInfo);

            // --- Sidebar ---
            panelSidebar = new Panel
            {
                Name = "panelSidebar",
                Dock = DockStyle.Left,
                Width = 230,
                BackColor = SidebarColor
            };

            btnPOS = MakeNavButton("btnPOS", "🛒 Bán hàng (POS)", btnPOS_Click);
            btnCategory = MakeNavButton("btnCategory", "📁 Quản lý Danh mục", btnCategory_Click);
            btnProduct = MakeNavButton("btnProduct", "📦 Quản lý Sản phẩm", btnProduct_Click);
            btnCustomer = MakeNavButton("btnCustomer", "👥 Quản lý Khách hàng", btnCustomer_Click);
            btnReports = MakeNavButton("btnReports", "📊 Báo cáo Doanh thu", btnReports_Click);
            btnUserManage = MakeNavButton("btnUserManage", "🛡 Quản trị Tài khoản", btnUserManage_Click);

            lblLogo = new Label
            {
                Name = "lblLogo",
                Text = "🍳 Kitchen Express",
                Dock = DockStyle.Top,
                Height = 70,
                ForeColor = Color.White,
                Font = new Font("Segoe UI Emoji", 14f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            panelUserFooter = new Panel
            {
                Name = "panelUserFooter",
                Dock = DockStyle.Bottom,
                Height = 60
            };
            btnLogout = new Button
            {
                Name = "btnLogout",
                Text = "🚪 Đăng xuất",
                Dock = DockStyle.Fill,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(192, 57, 43),
                Font = new Font("Segoe UI Emoji", 10f),
                Cursor = Cursors.Hand
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Click += btnLogout_Click;
            panelUserFooter.Controls.Add(btnLogout);

            // Với Dock = Top: control thêm SAU sẽ nằm TRÊN, nên thêm ngược từ dưới lên
            panelSidebar.Controls.Add(btnUserManage);
            panelSidebar.Controls.Add(btnReports);
            panelSidebar.Controls.Add(btnCustomer);
            panelSidebar.Controls.Add(btnProduct);
            panelSidebar.Controls.Add(btnCategory);
            panelSidebar.Controls.Add(btnPOS);
            panelSidebar.Controls.Add(lblLogo);
            panelSidebar.Controls.Add(panelUserFooter);

            // Thứ tự thêm vào form: Fill trước, Top, rồi Left sau cùng
            Controls.Add(panelMainContent);
            Controls.Add(panelTopHeader);
            Controls.Add(panelSidebar);

            ResumeLayout(false);
        }

        // ================= LOAD + PHÂN QUYỀN =================
        private void FormMainShell_Load(object sender, EventArgs e)
        {
            if (!RoleScreens.ContainsKey(SessionManager.CurrentRole))
            {
                MessageBox.Show("Tài khoản chưa được cấp quyền hạn hợp lệ!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
                return;
            }

            lblUserInfo.Text = "Nhân viên: " + SessionManager.CurrentUsername +
                               " | Vai trò: [" + SessionManager.CurrentRole + "]";

            // Lớp 1: ẩn nút theo vai trò
            btnPOS.Visible = Can("POS");
            btnCategory.Visible = Can("Category");
            btnProduct.Visible = Can("Product");
            btnCustomer.Visible = Can("Customer");
            btnReports.Visible = Can("Reports");
            btnUserManage.Visible = Can("Users");

            OpenDefaultScreenByRole();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // FormMainShell
            // 
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Name = "FormMainShell";
            this.Load += new System.EventHandler(this.FormMainShell_Load_1);
            this.ResumeLayout(false);

        }

        private void FormMainShell_Load_1(object sender, EventArgs e)
        {

        }

        // Màn hình mặc định theo bảng tổng kết cuối đề
        private void OpenDefaultScreenByRole()
        {
            switch (SessionManager.CurrentRole.ToUpper())
            {
                case "ADMIN": btnCategory_Click(null, null); break;
                case "CASHIER": btnPOS_Click(null, null); break;
                case "WAREHOUSE": btnProduct_Click(null, null); break;
            }
        }

        // ================= NHÚNG FORM CON =================
        private void OpenChildForm(Form childForm, string screenTitle, Button senderButton)
        {
            if (_activeForm != null) _activeForm.Close();

            HighlightActiveButton(senderButton);
            _activeForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;

            panelMainContent.Controls.Clear();
            panelMainContent.Controls.Add(childForm);
            panelMainContent.Tag = childForm;

            lblTitle.Text = screenTitle;
            childForm.BringToFront();
            childForm.Show();
        }

        // Lớp 2: chặn ở logic, kể cả khi ai đó gọi nút bằng code
        private void Open(string screen, Func<Form> create, string title, Button btn)
        {
            if (!Can(screen))
            {
                MessageBox.Show("Bạn không có quyền truy cập chức năng này!", "Từ chối",
                    MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            OpenChildForm(create(), title, btn);
        }

        private void HighlightActiveButton(Button activeButton)
        {
            foreach (Control ctrl in panelSidebar.Controls)
            {
                Button btn = ctrl as Button;
                if (btn != null) btn.BackColor = SidebarColor;
            }
            activeButton.BackColor = ActiveColor;
        }

        // ================= SỰ KIỆN CLICK =================
        private void btnPOS_Click(object sender, EventArgs e)
        {
            Open("POS", () => new FormPOS(),
      "HỆ THỐNG QUẦY BÁN HÀNG & THU NGÂN (POS)", btnPOS);
        }

        private void btnCategory_Click(object sender, EventArgs e)
        {
            Open("Category", () => new FormCategoryManagement(),
                 "QUẢN LÝ DANH MỤC NHÓM HÀNG", btnCategory);
        }

        private void btnProduct_Click(object sender, EventArgs e)
        {
            Open("Product", () => new FormProductManagement(),
        "QUẢN LÝ SẢN PHẨM & KHO HÀNG", btnProduct);
        }

        private void btnCustomer_Click(object sender, EventArgs e)
        {
            Open("Customer", () => new FormCustomerManagement(),
                 "QUẢN LÝ KHÁCH HÀNG & TÍCH ĐIỂM", btnCustomer);
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            Open("Reports", () => new FormQuickReport(),
     "BÁO CÁO DOANH THU & HIỆU SUẤT", btnReports);
        }

        private void btnUserManage_Click(object sender, EventArgs e)
        {
            Open("Users", () => new FormUserManagement(),
       "QUẢN TRỊ TÀI KHOẢN VÀ PHÂN QUYỀN", btnUserManage);
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                SessionManager.Clear();
                Close();   // FormLogin sẽ tự hiện lại
            }
        }
    }
}