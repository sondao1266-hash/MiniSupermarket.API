/*
 * Họ tên sinh viên: Nguyen Van Teo
 * Mã sinh viên: 123456789
 * Mô tả: Giao diện quản lý sản phẩm với phân trang LINQ, tìm kiếm, lọc theo nhóm hàng và CRUD hoàn chỉnh
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using MiniSupermarket.WinForms.Services;

namespace MiniSupermarket.WinForms
{
    // Lớp nhận dữ liệu DTO từ API về Client
    public class ProductItemViewDto
    {
        public int ProductId { get; set; }
        public string Barcode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }

    // Tương thích với FormPOS cũ
    public class ProductRow : ProductItemViewDto
    {
    }

    public class CategoryComboDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public override string ToString() => CategoryName;
    }

    public class PagedViewResult<T>
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public List<T> Items { get; set; } = new List<T>();
    }

    public partial class FormProductManagement : Form
    {
        private int _currentPage = 1;
        private readonly int _pageSize = 5;
        private int _totalPages = 1;

        // UI Controls
        private DataGridView dgvProducts;
        private TextBox txtSearch, txtId, txtBarcode, txtProductName;
        private ComboBox cboFilterCategory, cboCategory;
        private NumericUpDown nudPrice, nudStock;
        private Button btnSearch, btnLoad, btnCreate, btnUpdate, btnDelete, btnClear;
        private Button btnPrev, btnNext;
        private Label lblPaging;

        public FormProductManagement()
        {
            BuildUI();
            Load += FormProductManagement_Load;
        }

        private Button MakeButton(string text, Color color, EventHandler onClick)
        {
            var b = new Button
            {
                Text = text,
                Height = 36,
                Dock = DockStyle.Top,
                FlatStyle = FlatStyle.Flat,
                BackColor = color,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 5, 0, 0)
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
                Font = new Font("Segoe UI", 9f, FontStyle.Regular)
            });
            c.Location = new Point(15, y + 20);
            c.Width = p.Width - 30;
            c.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            p.Controls.Add(c);
            y += 56;
        }

        private void BuildUI()
        {
            SuspendLayout();
            Text = "Kitchen Express - Quản lý Hàng Hóa & Tồn Kho Nhà Bếp (Phân Trang)";
            BackColor = Color.FromArgb(244, 245, 247);
            ClientSize = new Size(1100, 650);
            StartPosition = FormStartPosition.CenterScreen;

            // 1. DataGridView ở giữa
            dgvProducts = new DataGridView
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
                BorderStyle = BorderStyle.None,
                RowTemplate = { Height = 28 }
            };
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ProductId", HeaderText = "ID", Width = 55 });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Barcode", HeaderText = "Mã vạch", Width = 130 });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ProductName", HeaderText = "Tên sản phẩm", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Price",
                HeaderText = "Đơn giá (VNĐ)",
                Width = 110,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "StockQuantity",
                HeaderText = "Tồn kho",
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CategoryName", HeaderText = "Nhóm hàng", Width = 180 });
            dgvProducts.SelectionChanged += dgvProducts_SelectionChanged;

            // 2. Khung phân trang ở dưới DataGridView
            var panelPaging = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 45,
                BackColor = Color.White
            };
            btnPrev = new Button
            {
                Text = "<< Trang trước",
                Location = new Point(15, 8),
                Size = new Size(110, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(41, 128, 185),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold)
            };
            btnPrev.FlatAppearance.BorderSize = 0;
            btnPrev.Click += btnPrev_Click;

            lblPaging = new Label
            {
                Text = "Trang 1 / 1 (Tổng: 0 SP)",
                Location = new Point(140, 14),
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80)
            };

            btnNext = new Button
            {
                Text = "Trang sau >>",
                Location = new Point(340, 8),
                Size = new Size(110, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(41, 128, 185),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold)
            };
            btnNext.FlatAppearance.BorderSize = 0;
            btnNext.Click += btnNext_Click;

            panelPaging.Controls.Add(btnPrev);
            panelPaging.Controls.Add(lblPaging);
            panelPaging.Controls.Add(btnNext);

            // Container chứa DataGridView + Paging Bar
            var panelGridContainer = new Panel { Dock = DockStyle.Fill };
            panelGridContainer.Controls.Add(dgvProducts);
            panelGridContainer.Controls.Add(panelPaging);

            // 3. Khung chi tiết & nút thao tác (bên phải)
            var panelDetail = new Panel
            {
                Dock = DockStyle.Right,
                Width = 340,
                BackColor = Color.White,
                Padding = new Padding(10)
            };
            int y = 15;
            txtId = new TextBox { ReadOnly = true, BackColor = Color.WhiteSmoke };
            txtBarcode = new TextBox();
            txtProductName = new TextBox();
            nudPrice = new NumericUpDown { Maximum = 1000000000, ThousandsSeparator = true, Increment = 1000 };
            nudStock = new NumericUpDown { Maximum = 1000000 };
            cboCategory = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };

            AddField(panelDetail, "Mã sản phẩm (Tự tăng):", txtId, ref y);
            AddField(panelDetail, "Mã vạch (Barcode):", txtBarcode, ref y);
            AddField(panelDetail, "Tên sản phẩm:", txtProductName, ref y);
            AddField(panelDetail, "Đơn giá (VNĐ):", nudPrice, ref y);
            AddField(panelDetail, "Số lượng tồn kho:", nudStock, ref y);
            AddField(panelDetail, "Nhóm hàng:", cboCategory, ref y);

            var panelButtons = new Panel { Dock = DockStyle.Bottom, Height = 190, Padding = new Padding(15, 0, 15, 10) };
            btnClear = MakeButton("LÀM MỚI FORM", Color.FromArgb(127, 140, 141), btnClear_Click);
            btnDelete = MakeButton("XÓA HÀNG (DELETE)", Color.FromArgb(192, 57, 43), btnDelete_Click);
            btnUpdate = MakeButton("CẬP NHẬT (UPDATE)", Color.FromArgb(41, 100, 180), btnUpdate_Click);
            btnCreate = MakeButton("THÊM MỚI (CREATE)", Color.FromArgb(39, 174, 96), btnCreate_Click);
            panelButtons.Controls.Add(btnClear);
            panelButtons.Controls.Add(btnDelete);
            panelButtons.Controls.Add(btnUpdate);
            panelButtons.Controls.Add(btnCreate);
            panelDetail.Controls.Add(panelButtons);

            // 4. Thanh tìm kiếm & lọc nhóm hàng ở trên (Top Panel)
            var panelTop = new Panel { Dock = DockStyle.Top, Height = 55, BackColor = Color.White };
            panelTop.Controls.Add(new Label { Text = "Tìm kiếm:", Location = new Point(15, 19), AutoSize = true, Font = new Font("Segoe UI", 9f) });
            txtSearch = new TextBox { Location = new Point(80, 16), Width = 190 };
            txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) btnSearch.PerformClick(); };
            panelTop.Controls.Add(txtSearch);

            panelTop.Controls.Add(new Label { Text = "Nhóm hàng:", Location = new Point(285, 19), AutoSize = true, Font = new Font("Segoe UI", 9f) });
            cboFilterCategory = new ComboBox { Location = new Point(365, 16), Width = 210, DropDownStyle = ComboBoxStyle.DropDownList };
            cboFilterCategory.SelectedIndexChanged += (s, e) => { _currentPage = 1; _ = LoadProductsPagedAsync(); };
            panelTop.Controls.Add(cboFilterCategory);

            btnSearch = new Button
            {
                Text = "🔍 Tìm",
                Location = new Point(590, 14),
                Size = new Size(80, 28),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(41, 100, 180),
                ForeColor = Color.White
            };
            btnSearch.FlatAppearance.BorderSize = 0;
            btnSearch.Click += btnSearch_Click;
            panelTop.Controls.Add(btnSearch);

            btnLoad = new Button
            {
                Text = "⟳ Tải lại",
                Location = new Point(680, 14),
                Size = new Size(85, 28),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White
            };
            btnLoad.FlatAppearance.BorderSize = 0;
            btnLoad.Click += btnLoad_Click;
            panelTop.Controls.Add(btnLoad);

            // Thêm các Panel vào Form (thứ tự Dock)
            Controls.Add(panelGridContainer);
            Controls.Add(panelDetail);
            Controls.Add(panelTop);

            ResumeLayout(false);
        }

        // ================= XỬ LÝ SỰ KIỆN & GỌI API =================

        private async void FormProductManagement_Load(object sender, EventArgs e)
        {
            // Đồng bộ token nếu đã đăng nhập qua SessionManager
            if (!string.IsNullOrEmpty(SessionManager.JwtToken))
            {
                ApiClientService.AttachToken(SessionManager.JwtToken);
            }

            await LoadCategoriesComboAsync();
            await LoadProductsPagedAsync();
        }

        // 1. Tải danh sách nhóm hàng lên ComboBox
        private async Task LoadCategoriesComboAsync()
        {
            try
            {
                var categories = await ApiClientService.Client
                    .GetFromJsonAsync<List<CategoryComboDto>>("categories");

                if (categories != null)
                {
                    // ComboBox nhập liệu chi tiết
                    cboCategory.DataSource = new List<CategoryComboDto>(categories);
                    cboCategory.DisplayMember = "CategoryName";
                    cboCategory.ValueMember = "CategoryId";

                    // ComboBox bộ lọc (Bài tập 6.2)
                    var filterList = new List<CategoryComboDto>
                    {
                        new CategoryComboDto { CategoryId = 0, CategoryName = "-- Tất cả nhóm hàng --" }
                    };
                    filterList.AddRange(categories);
                    cboFilterCategory.DataSource = filterList;
                    cboFilterCategory.DisplayMember = "CategoryName";
                    cboFilterCategory.ValueMember = "CategoryId";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh mục: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 2. Gọi API phân trang và hiển thị dữ liệu lên DataGridView
        private async Task LoadProductsPagedAsync()
        {
            try
            {
                string search = txtSearch.Text.Trim();
                int catId = 0;
                if (cboFilterCategory.SelectedValue is int cVal) catId = cVal;

                string url = $"products/paged?keyword={Uri.EscapeDataString(search)}&pageNumber={_currentPage}&pageSize={_pageSize}";
                if (catId > 0)
                {
                    url += $"&categoryId={catId}";
                }

                var result = await ApiClientService.Client.GetFromJsonAsync<PagedViewResult<ProductItemViewDto>>(url);
                if (result != null)
                {
                    _currentPage = result.CurrentPage;
                    _totalPages = result.TotalPages == 0 ? 1 : result.TotalPages;

                    dgvProducts.DataSource = result.Items;
                    lblPaging.Text = $"Trang {_currentPage} / {_totalPages} (Tổng: {result.TotalItems} SP)";

                    btnPrev.Enabled = _currentPage > 1;
                    btnNext.Enabled = _currentPage < _totalPages;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lấy dữ liệu sản phẩm: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 3. Sự kiện chuyển trang
        private async void btnPrev_Click(object sender, EventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                await LoadProductsPagedAsync();
            }
        }

        private async void btnNext_Click(object sender, EventArgs e)
        {
            if (_currentPage < _totalPages)
            {
                _currentPage++;
                await LoadProductsPagedAsync();
            }
        }

        // 4. Sự kiện tìm kiếm
        private async void btnSearch_Click(object sender, EventArgs e)
        {
            _currentPage = 1;
            await LoadProductsPagedAsync();
        }

        private async void btnLoad_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            if (cboFilterCategory.Items.Count > 0) cboFilterCategory.SelectedIndex = 0;
            _currentPage = 1;
            await LoadProductsPagedAsync();
        }

        // 5. Chọn dòng trên DataGridView
        private void dgvProducts_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvProducts.CurrentRow == null) return;
            var p = dgvProducts.CurrentRow.DataBoundItem as ProductItemViewDto;
            if (p == null) return;

            txtId.Text = p.ProductId.ToString();
            txtBarcode.Text = p.Barcode;
            txtProductName.Text = p.ProductName;
            nudPrice.Value = Math.Min(nudPrice.Maximum, p.Price);
            nudStock.Value = Math.Min(nudStock.Maximum, p.StockQuantity);
            if (p.CategoryId > 0)
            {
                cboCategory.SelectedValue = p.CategoryId;
            }
        }

        // 6. Thêm mới sản phẩm qua API
        private async void btnCreate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBarcode.Text))
            {
                MessageBox.Show("Vui lòng nhập mã vạch!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtProductName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên sản phẩm!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var newProd = new
            {
                Barcode = txtBarcode.Text.Trim(),
                ProductName = txtProductName.Text.Trim(),
                Price = nudPrice.Value,
                StockQuantity = (int)nudStock.Value,
                CategoryId = Convert.ToInt32(cboCategory.SelectedValue)
            };

            var res = await ApiClientService.Client.PostAsJsonAsync("products", newProd);
            if (res.IsSuccessStatusCode)
            {
                MessageBox.Show("Thêm mới sản phẩm thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm();
                await LoadProductsPagedAsync();
            }
            else
            {
                string msg = await res.Content.ReadAsStringAsync();
                MessageBox.Show($"Thêm thất bại. Mã lỗi: {res.StatusCode}\n{msg}", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // 7. Cập nhật sản phẩm qua API
        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtId.Text) || !int.TryParse(txtId.Text, out int id))
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần cập nhật từ danh sách!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var updateProd = new
            {
                Barcode = txtBarcode.Text.Trim(),
                ProductName = txtProductName.Text.Trim(),
                Price = nudPrice.Value,
                StockQuantity = (int)nudStock.Value,
                CategoryId = Convert.ToInt32(cboCategory.SelectedValue)
            };

            var res = await ApiClientService.Client.PutAsJsonAsync($"products/{id}", updateProd);
            if (res.IsSuccessStatusCode)
            {
                MessageBox.Show("Cập nhật sản phẩm thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadProductsPagedAsync();
            }
            else
            {
                string msg = await res.Content.ReadAsStringAsync();
                MessageBox.Show($"Cập nhật thất bại. Mã lỗi: {res.StatusCode}\n{msg}", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // 8. Xóa sản phẩm qua API
        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvProducts.CurrentRow == null) return;
            int id = Convert.ToInt32(dgvProducts.CurrentRow.Cells[0].Value);
            var confirm = MessageBox.Show($"Bạn có chắc chắn muốn xóa sản phẩm #{id}?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                var res = await ApiClientService.Client.DeleteAsync($"products/{id}");
                if (res.IsSuccessStatusCode)
                {
                    MessageBox.Show("Đã xóa sản phẩm thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                    await LoadProductsPagedAsync();
                }
                else
                {
                    string msg = await res.Content.ReadAsStringAsync();
                    MessageBox.Show($"Xóa thất bại: {msg}", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            txtId.Clear();
            txtBarcode.Clear();
            txtProductName.Clear();
            nudPrice.Value = 0;
            nudStock.Value = 0;
            if (cboCategory.Items.Count > 0) cboCategory.SelectedIndex = 0;
            txtBarcode.Focus();
        }
    }
}
