using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniSupermarket.WinForms
{
    public class UserRow
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
    }

    public class FormUserManagement : Form
    {
        private DataGridView dgvUsers;
        private TextBox txtUsername, txtFullName, txtPassword;
        private ComboBox cboRole;
        private Button btnAddUser, btnResetPassword, btnToggleLock;
        private Label lblSelected;

        private UserRow _selected = null;

        public FormUserManagement()
        {
            BuildUI();
            Load += FormUserManagement_Load;
        }

        // ================= DỰNG GIAO DIỆN =================
        private Button MakeButton(string text, Color color, EventHandler onClick)
        {
            var b = new Button
            {
                Text = text,
                Height = 38,
                Dock = DockStyle.Top,
                FlatStyle = FlatStyle.Flat,
                BackColor = color,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderSize = 0;
            b.Click += onClick;
            return b;
        }

        private void AddField(Panel p, string label, Control c, ref int y)
        {
            p.Controls.Add(new Label
            {
                Text = label,
                Location = new Point(15, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9f)
            });
            c.Location = new Point(15, y + 20);
            c.Width = p.Width - 30;
            c.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            p.Controls.Add(c);
            y += 58;
        }

        private void BuildUI()
        {
            SuspendLayout();
            Text = "Quản trị Tài khoản";
            BackColor = Color.FromArgb(244, 245, 247);
            ClientSize = new Size(1000, 600);

            // ---- Bảng nhân viên (giữa) ----
            dgvUsers = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                BorderStyle = BorderStyle.None
            };
            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "ID", Width = 50 });
            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Username", HeaderText = "Tên đăng nhập", Width = 150 });
            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "FullName", HeaderText = "Họ và tên", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Role", HeaderText = "Vai trò", Width = 110 });
            dgvUsers.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = "IsActive", HeaderText = "Hoạt động", Width = 85 });
            dgvUsers.SelectionChanged += dgvUsers_SelectionChanged;
            dgvUsers.CellFormatting += dgvUsers_CellFormatting;

            // ---- Khung bên phải ----
            var panelRight = new Panel { Dock = DockStyle.Right, Width = 330, BackColor = Color.White };

            // Nút thao tác trên tài khoản đang chọn (dưới cùng)
            var panelButtons = new Panel { Dock = DockStyle.Bottom, Height = 160, Padding = new Padding(15, 5, 15, 10) };
            lblSelected = new Label
            {
                Text = "Chưa chọn tài khoản nào",
                Dock = DockStyle.Top,
                Height = 30,
                ForeColor = Color.DimGray,
                UseMnemonic = false
            };
            btnToggleLock = MakeButton("KHÓA / MỞ KHÓA", Color.FromArgb(192, 57, 43), btnToggleLock_Click);
            btnResetPassword = MakeButton("ĐẶT LẠI MẬT KHẨU", Color.FromArgb(230, 126, 34), btnResetPassword_Click);
            btnToggleLock.Margin = new Padding(0, 6, 0, 0);
            // Dock Top: thêm ngược từ dưới lên
            panelButtons.Controls.Add(btnToggleLock);
            panelButtons.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 8 });
            panelButtons.Controls.Add(btnResetPassword);
            panelButtons.Controls.Add(lblSelected);

            // Form thêm tài khoản
            txtUsername = new TextBox();
            txtFullName = new TextBox();
            txtPassword = new TextBox { UseSystemPasswordChar = true };
            cboRole = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            cboRole.Items.AddRange(new object[] { "Admin", "Cashier", "Warehouse" });
            cboRole.SelectedIndex = 1;

            int y = 15;
            AddField(panelRight, "Tên đăng nhập", txtUsername, ref y);
            AddField(panelRight, "Họ và tên", txtFullName, ref y);
            AddField(panelRight, "Mật khẩu (thêm mới / đặt lại, tối thiểu 6 ký tự)", txtPassword, ref y);
            AddField(panelRight, "Vai trò", cboRole, ref y);

            btnAddUser = new Button
            {
                Text = "THÊM TÀI KHOẢN",
                Location = new Point(15, y + 5),
                Size = new Size(300, 38),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            btnAddUser.FlatAppearance.BorderSize = 0;
            btnAddUser.Click += btnAddUser_Click;
            panelRight.Controls.Add(btnAddUser);

            panelRight.Controls.Add(panelButtons);

            Controls.Add(dgvUsers);
            Controls.Add(panelRight);

            ResumeLayout(false);
        }

        // ================= GỌI API =================
        private async void FormUserManagement_Load(object sender, EventArgs e)
        {
            await LoadUsersAsync();
        }

        private async Task LoadUsersAsync()
        {
            try
            {
                var res = await ApiClient.GetClient().GetAsync("users");
                if (this.IsDisposed) return;
                if (!await ApiClient.CheckAsync(res)) return;

                var users = await res.Content.ReadFromJsonAsync<List<UserRow>>();
                if (this.IsDisposed) return;
                dgvUsers.DataSource = users;
            }
            catch (Exception ex)
            {
                if (this.IsDisposed) return;
                MessageBox.Show("Lỗi lấy danh sách tài khoản: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Hiển thị khóa: dòng bị khóa có chữ xám
        private void dgvUsers_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var u = dgvUsers.Rows[e.RowIndex].DataBoundItem as UserRow;
            if (u != null && !u.IsActive)
            {
                e.CellStyle.ForeColor = Color.Gray;
                e.CellStyle.BackColor = Color.FromArgb(240, 240, 240);
            }
        }

        private void dgvUsers_SelectionChanged(object sender, EventArgs e)
        {
            _selected = dgvUsers.CurrentRow != null ? dgvUsers.CurrentRow.DataBoundItem as UserRow : null;
            lblSelected.Text = _selected == null
                ? "Chưa chọn tài khoản nào"
                : "Đang chọn: " + _selected.Username + (_selected.IsActive ? "" : " (đã khóa)");
        }

        private async void btnAddUser_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtFullName.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Vui lòng nhập đủ tên đăng nhập, họ tên và mật khẩu!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (txtPassword.Text.Length < 6)
            {
                MessageBox.Show("Mật khẩu tối thiểu 6 ký tự!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var newUser = new
                {
                    Username = txtUsername.Text.Trim(),
                    FullName = txtFullName.Text.Trim(),
                    Password = txtPassword.Text,
                    Role = cboRole.SelectedItem.ToString()
                };
                var res = await ApiClient.GetClient().PostAsJsonAsync("users", newUser);
                if (this.IsDisposed) return;
                if (!await ShowApiResultAsync(res)) return;

                MessageBox.Show("Tạo tài khoản mới thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtUsername.Clear();
                txtFullName.Clear();
                txtPassword.Clear();
                await LoadUsersAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnResetPassword_Click(object sender, EventArgs e)
        {
            if (_selected == null)
            {
                MessageBox.Show("Hãy chọn một tài khoản trong bảng!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string newPassword = txtPassword.Text;
            string hint = string.IsNullOrWhiteSpace(newPassword)
                ? "về mặc định (123456)"
                : "thành mật khẩu vừa nhập";
            var confirm = MessageBox.Show("Đặt lại mật khẩu của \"" + _selected.Username + "\" " + hint + "?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                var res = await ApiClient.GetClient().PutAsJsonAsync(
                    "users/" + _selected.Id + "/reset-password", new { NewPassword = newPassword });
                if (this.IsDisposed) return;
                if (!await ShowApiResultAsync(res)) return;

                txtPassword.Clear();
                MessageBox.Show("Đã đặt lại mật khẩu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnToggleLock_Click(object sender, EventArgs e)
        {
            if (_selected == null)
            {
                MessageBox.Show("Hãy chọn một tài khoản trong bảng!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var res = await ApiClient.GetClient().PutAsync("users/" + _selected.Id + "/toggle-lock", null);
                if (this.IsDisposed) return;
                if (!await ShowApiResultAsync(res)) return;

                await LoadUsersAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Hiện đúng thông báo lỗi của API (VD: "Tên đăng nhập đã tồn tại!")
        private async Task<bool> ShowApiResultAsync(System.Net.Http.HttpResponseMessage res)
        {
            if (res.IsSuccessStatusCode) return true;

            if (res.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                string body = await res.Content.ReadAsStringAsync();
                string msg = "Yêu cầu không hợp lệ.";
                try
                {
                    using (var doc = System.Text.Json.JsonDocument.Parse(body))
                    {
                        System.Text.Json.JsonElement m;
                        if (doc.RootElement.TryGetProperty("message", out m)) msg = m.GetString();
                    }
                }
                catch { }
                MessageBox.Show(msg, "Thất bại", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return await ApiClient.CheckAsync(res);
        }
    }
}