using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniSupermarket.WinForms
{
    public partial class FormCategoryManagement : Form
    {
        // Danh sách nhóm hàng tải từ Web API (SQL Server)
        private List<Category> categories = new List<Category>();

        public FormCategoryManagement()
        {
            InitializeComponent();
        }

        // =====================================================
        // FORM LOAD
        // =====================================================
        private async void FormCategoryManagement_Load_1(object sender, EventArgs e)
        {
            txtSearch.Text = "Nhập từ khóa...";
            txtSearch.ForeColor = Color.Gray;

            // Phân quyền giao diện: chỉ Admin được thêm / sửa / xóa
            bool isAdmin = SessionManager.CurrentRole == "Admin";
            btnAdd.Enabled = isAdmin;
            btnUpdate.Enabled = isAdmin;
            btnDelete.Enabled = isAdmin;
            this.Text = this.Text + " - " + SessionManager.CurrentRole;

            // Nút mở form quản lý khách hàng (tạo bằng code, đặt dưới nút Xóa)
            Button btnCustomers = new Button();
            btnCustomers.Text = "Khách hàng";
            btnCustomers.Name = "btnCustomers";
            btnCustomers.Size = btnDelete.Size;
            btnCustomers.Location = new Point(btnDelete.Left, btnDelete.Bottom + 10);
            btnCustomers.Click += btnCustomers_Click;
            btnDelete.Parent.Controls.Add(btnCustomers);

            await LoadDataAsync();
            SelectFirstRow();
            lblStatus.Text = "Ready";
        }

        private void btnCustomers_Click(object sender, EventArgs e)
        {
            using (FormCustomerManagement f = new FormCustomerManagement())
            {
                f.ShowDialog();
            }
        }

        // =====================================================
        // TẢI DỮ LIỆU TỪ API (GET /api/categories)
        // =====================================================
        private async Task LoadDataAsync()
        {
            try
            {
                HttpResponseMessage response = await ApiClient.GetClient().GetAsync("categories");
                if (!await ApiClient.CheckAsync(response))
                {
                    return;
                }
                categories = await response.Content.ReadFromJsonAsync<List<Category>>()
                             ?? new List<Category>();
                DisplayCategories(categories);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi quyền truy cập hoặc mất kết nối: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =====================================================
        // HIỂN THỊ DANH SÁCH
        // =====================================================
        private void DisplayCategories(List<Category> list)
        {
            // Nếu lưới chưa có cột (Designer bị mất cột) thì tạo lại bằng code
            if (dataGridView1.Columns.Count == 0)
            {
                dataGridView1.Columns.Add("colID", "Mã ID");
                dataGridView1.Columns.Add("colName", "Tên nhóm hàng");
                dataGridView1.Columns.Add("colDescription", "Mô tả");
                dataGridView1.Columns["colDescription"].AutoSizeMode =
                    DataGridViewAutoSizeColumnMode.Fill;
            }

            dataGridView1.Rows.Clear();
            foreach (Category category in list)
            {
                int rowIndex = dataGridView1.Rows.Add();
                DataGridViewRow row = dataGridView1.Rows[rowIndex];
                row.Cells[0].Value = category.CategoryId;
                row.Cells[1].Value = category.CategoryName;
                row.Cells[2].Value = category.Description;
            }

            lblStatus.Text = "Có " + list.Count + " nhóm hàng";
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            ShowCategory(e.RowIndex);
        }

        private void ShowCategory(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= dataGridView1.Rows.Count)
            {
                return;
            }

            DataGridViewRow row = dataGridView1.Rows[rowIndex];
            txtID.Text = Convert.ToString(row.Cells[0].Value);
            txtName.Text = Convert.ToString(row.Cells[1].Value);
            txtDescription.Text = Convert.ToString(row.Cells[2].Value);
        }

        private void SelectFirstRow()
        {
            if (dataGridView1.Rows.Count > 0)
            {
                dataGridView1.ClearSelection();
                dataGridView1.Rows[0].Selected = true;
                dataGridView1.CurrentCell = dataGridView1.Rows[0].Cells[0];
                ShowCategory(0);
            }
            else
            {
                ClearInput();
            }
        }

        private void ClearInput()
        {
            txtID.Clear();
            txtName.Clear();
            txtDescription.Clear();
        }

        // =====================================================
        // TÌM KIẾM (GET /api/categories/search?keyword=...)
        // =====================================================
        private async void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();

            if (string.IsNullOrWhiteSpace(keyword) || keyword == "Nhập từ khóa...")
            {
                await LoadDataAsync();
                SelectFirstRow();
                return;
            }

            try
            {
                HttpResponseMessage response = await ApiClient.GetClient()
                    .GetAsync("categories/search?keyword=" + Uri.EscapeDataString(keyword));
                if (!await ApiClient.CheckAsync(response))
                {
                    return;
                }
                List<Category> result = await response.Content.ReadFromJsonAsync<List<Category>>()
                                        ?? new List<Category>();

                DisplayCategories(result);

                if (result.Count > 0)
                {
                    SelectFirstRow();
                    lblStatus.Text = "Tìm thấy " + result.Count + " nhóm hàng";
                }
                else
                {
                    ClearInput();
                    lblStatus.Text = "Không tìm thấy";
                    MessageBox.Show("Không tìm thấy nhóm hàng!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =====================================================
        // TẢI LẠI
        // =====================================================
        private async void btnReload_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "Nhập từ khóa...";
            txtSearch.ForeColor = Color.Gray;

            await LoadDataAsync();
            SelectFirstRow();
            lblStatus.Text = "Đã tải lại dữ liệu";
        }

        // =====================================================
        // THÊM MỚI (POST /api/categories)
        // =====================================================
        private async void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Vui lòng nhập Tên Nhóm hàng!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return;
            }

            try
            {
                Category category = new Category
                {
                    CategoryName = txtName.Text.Trim(),
                    Description = txtDescription.Text.Trim()
                };

                HttpResponseMessage response = await ApiClient.GetClient()
                    .PostAsJsonAsync("categories", category);
                if (!await ApiClient.CheckAsync(response))
                {
                    return;
                }
                Category created = await response.Content.ReadFromJsonAsync<Category>();

                await LoadDataAsync();
                if (created != null)
                {
                    SelectCategoryByID(created.CategoryId);
                }

                lblStatus.Text = "Đã thêm nhóm hàng";
                MessageBox.Show("Thêm mới thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =====================================================
        // CẬP NHẬT (PUT /api/categories/{id})
        // =====================================================
        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            int id;

            if (!int.TryParse(txtID.Text, out id))
            {
                MessageBox.Show("Vui lòng chọn nhóm hàng cần cập nhật!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Vui lòng nhập Tên Nhóm hàng!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return;
            }

            try
            {
                Category category = new Category
                {
                    CategoryId = id,
                    CategoryName = txtName.Text.Trim(),
                    Description = txtDescription.Text.Trim()
                };

                HttpResponseMessage response = await ApiClient.GetClient()
                    .PutAsJsonAsync("categories/" + id, category);
                if (!await ApiClient.CheckAsync(response))
                {
                    return;
                }

                await LoadDataAsync();
                SelectCategoryByID(id);

                lblStatus.Text = "Đã cập nhật nhóm hàng";
                MessageBox.Show("Cập nhật thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =====================================================
        // XÓA (DELETE /api/categories/{id})
        // =====================================================
        private async void btnDelete_Click(object sender, EventArgs e)
        {
            int id;

            if (!int.TryParse(txtID.Text, out id))
            {
                MessageBox.Show("Vui lòng chọn nhóm hàng cần xóa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                "Bạn có chắc muốn xóa nhóm hàng \"" + txtName.Text + "\" không?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                return;
            }

            try
            {
                HttpResponseMessage response = await ApiClient.GetClient().DeleteAsync("categories/" + id);
                if (!await ApiClient.CheckAsync(response))
                {
                    return;
                }

                await LoadDataAsync();
                SelectFirstRow();

                lblStatus.Text = "Đã xóa nhóm hàng";
                MessageBox.Show("Xóa thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SelectCategoryByID(int id)
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                string currentID = Convert.ToString(dataGridView1.Rows[i].Cells[0].Value);

                if (currentID == id.ToString())
                {
                    dataGridView1.ClearSelection();
                    dataGridView1.Rows[i].Selected = true;
                    dataGridView1.CurrentCell = dataGridView1.Rows[i].Cells[0];
                    ShowCategory(i);
                    return;
                }
            }
        }

        // =====================================================
        // PLACEHOLDER Ô TÌM KIẾM
        // =====================================================
        private void txtSearch_Enter(object sender, EventArgs e)
        {
            if (txtSearch.Text == "Nhập từ khóa...")
            {
                txtSearch.Clear();
                txtSearch.ForeColor = Color.Black;
            }
        }

        private void txtSearch_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                txtSearch.Text = "Nhập từ khóa...";
                txtSearch.ForeColor = Color.Gray;
            }
        }

        // =====================================================
        // MODEL CATEGORY (khớp JSON của API)
        // =====================================================
        public class Category
        {
            public int CategoryId { get; set; }

            public string CategoryName { get; set; }

            public string Description { get; set; }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
