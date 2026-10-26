using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniSupermarket.WinForms
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public int StockQuantity { get; set; }
        public decimal TotalPrice { get { return UnitPrice * Quantity; } }
    }

    public class PosCustomer
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public int RewardPoints { get; set; }
        public string MembershipRank { get; set; }
    }

    public class CheckoutResult
    {
        public int OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ChangeAmount { get; set; }
        public int PointsEarned { get; set; }
        public int? CustomerPoints { get; set; }
        public string MembershipRank { get; set; }
    }

    public class FormPOS : Form
    {
        private readonly List<CartItem> _cart = new List<CartItem>();
        private BindingSource _bs;
        private DataGridView dgvCart;
        private TextBox txtBarcode, txtCustomerPhone, txtCashReceived;
        private Label lblCustomerName, lblTotalAmount, lblChange, lblPoints;
        private Button btnFindCustomer, btnCheckout, btnClearCart, btnInc, btnDec, btnRemove;
        private PosCustomer _customer = null;

        public FormPOS()
        {
            BuildUI();
            Load += delegate { txtBarcode.Focus(); };
        }

        // ================= DỰNG GIAO DIỆN =================
        private Button MakeButton(string text, Color color, Point loc, Size size, EventHandler onClick)
        {
            var b = new Button
            {
                Text = text,
                Location = loc,
                Size = size,
                FlatStyle = FlatStyle.Flat,
                BackColor = color,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                UseMnemonic = false
            };
            b.FlatAppearance.BorderSize = 0;
            b.Click += onClick;
            return b;
        }

        private Label MakeLabel(string text, Point loc, Size size, Font font, ContentAlignment align, Color color)
        {
            return new Label
            {
                Text = text,
                Location = loc,
                Size = size,
                Font = font,
                TextAlign = align,
                ForeColor = color,
                UseMnemonic = false
            };
        }

        private void BuildUI()
        {
            SuspendLayout();
            Text = "Bán hàng (POS)";
            BackColor = Color.FromArgb(244, 245, 247);
            ClientSize = new Size(1000, 600);

            // ===== KHU VỰC TRÁI: giỏ hàng =====
            var panelLeft = new Panel { Dock = DockStyle.Fill };

            var panelTop = new Panel { Dock = DockStyle.Top, Height = 105, BackColor = Color.White };
            panelTop.Controls.Add(MakeLabel("Quét mã vạch hoặc nhập tên sản phẩm, rồi nhấn Enter:",
                new Point(15, 10), new Size(520, 22), new Font("Segoe UI", 10f), ContentAlignment.MiddleLeft, Color.Black));
            txtBarcode = new TextBox { Location = new Point(15, 36), Width = 520, Font = new Font("Segoe UI", 14f) };
            txtBarcode.KeyDown += txtBarcode_KeyDown;
            panelTop.Controls.Add(txtBarcode);
            panelTop.Controls.Add(MakeLabel("Mã vạch mẫu: 893000000001 ... 893000000015",
                new Point(15, 78), new Size(520, 20), new Font("Segoe UI", 9f), ContentAlignment.MiddleLeft, Color.Gray));

            var panelBottom = new Panel { Dock = DockStyle.Bottom, Height = 58, BackColor = Color.White };
            btnInc = MakeButton("+ Tăng SL", Color.FromArgb(41, 100, 180), new Point(15, 10), new Size(120, 36), btnInc_Click);
            btnDec = MakeButton("− Giảm SL", Color.FromArgb(108, 117, 125), new Point(145, 10), new Size(120, 36), btnDec_Click);
            btnRemove = MakeButton("Xóa dòng", Color.FromArgb(192, 57, 43), new Point(275, 10), new Size(120, 36), btnRemove_Click);
            panelBottom.Controls.Add(btnInc);
            panelBottom.Controls.Add(btnDec);
            panelBottom.Controls.Add(btnRemove);

            dgvCart = new DataGridView
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
                Font = new Font("Segoe UI", 10f)
            };
            var right = new DataGridViewCellStyle { Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight };
            dgvCart.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ProductId", HeaderText = "Mã SP", Width = 70 });
            dgvCart.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ProductName", HeaderText = "Tên sản phẩm", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvCart.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "UnitPrice", HeaderText = "Đơn giá", Width = 110, DefaultCellStyle = right });
            dgvCart.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Quantity", HeaderText = "SL", Width = 60 });
            dgvCart.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TotalPrice", HeaderText = "Thành tiền", Width = 130, DefaultCellStyle = right });

            _bs = new BindingSource { DataSource = _cart };
            dgvCart.DataSource = _bs;

            panelLeft.Controls.Add(dgvCart);
            panelLeft.Controls.Add(panelBottom);
            panelLeft.Controls.Add(panelTop);

            // ===== KHU VỰC PHẢI: khách hàng & thanh toán =====
            var panelRight = new Panel { Dock = DockStyle.Right, Width = 400, BackColor = Color.White };

            panelRight.Controls.Add(MakeLabel("Số điện thoại khách hàng thân thiết:",
                new Point(15, 12), new Size(370, 22), new Font("Segoe UI", 9.5f), ContentAlignment.MiddleLeft, Color.Black));
            txtCustomerPhone = new TextBox { Location = new Point(15, 38), Width = 250, Font = new Font("Segoe UI", 12f) };
            txtCustomerPhone.KeyDown += txtCustomerPhone_KeyDown;
            panelRight.Controls.Add(txtCustomerPhone);
            btnFindCustomer = MakeButton("Tìm", Color.FromArgb(41, 100, 180), new Point(275, 36), new Size(110, 32), btnFindCustomer_Click);
            panelRight.Controls.Add(btnFindCustomer);

            lblCustomerName = MakeLabel("Khách vãng lai (không tích điểm)",
                new Point(15, 76), new Size(370, 44), new Font("Segoe UI", 10f, FontStyle.Italic), ContentAlignment.MiddleLeft, Color.DimGray);
            panelRight.Controls.Add(lblCustomerName);

            panelRight.Controls.Add(MakeLabel("TỔNG TIỀN CẦN THANH TOÁN",
                new Point(15, 130), new Size(370, 22), new Font("Segoe UI", 9.5f, FontStyle.Bold), ContentAlignment.MiddleLeft, Color.Black));
            lblTotalAmount = MakeLabel("0 đ", new Point(15, 154), new Size(370, 56),
                new Font("Segoe UI", 26f, FontStyle.Bold), ContentAlignment.MiddleRight, Color.FromArgb(192, 57, 43));
            panelRight.Controls.Add(lblTotalAmount);

            lblPoints = MakeLabel("Điểm tích lũy đơn này: 0 (1 điểm / 10.000đ)",
                new Point(15, 212), new Size(370, 22), new Font("Segoe UI", 9f), ContentAlignment.MiddleLeft, Color.Gray);
            panelRight.Controls.Add(lblPoints);

            panelRight.Controls.Add(MakeLabel("Tiền khách đưa (đ):",
                new Point(15, 250), new Size(370, 22), new Font("Segoe UI", 9.5f), ContentAlignment.MiddleLeft, Color.Black));
            txtCashReceived = new TextBox { Location = new Point(15, 276), Width = 370, Font = new Font("Segoe UI", 14f), TextAlign = HorizontalAlignment.Right };
            txtCashReceived.TextChanged += delegate { CalculateChange(); };
            panelRight.Controls.Add(txtCashReceived);

            panelRight.Controls.Add(MakeLabel("Tiền thừa trả khách:",
                new Point(15, 322), new Size(370, 22), new Font("Segoe UI", 9.5f), ContentAlignment.MiddleLeft, Color.Black));
            lblChange = MakeLabel("0 đ", new Point(15, 346), new Size(370, 44),
                new Font("Segoe UI", 18f, FontStyle.Bold), ContentAlignment.MiddleRight, Color.FromArgb(39, 174, 96));
            panelRight.Controls.Add(lblChange);

            btnCheckout = MakeButton("THANH TOÁN (F9)", Color.FromArgb(30, 132, 73), new Point(15, 410), new Size(370, 56), btnCheckout_Click);
            btnCheckout.Font = new Font("Segoe UI", 13f, FontStyle.Bold);
            panelRight.Controls.Add(btnCheckout);

            btnClearCart = MakeButton("HỦY GIỎ HÀNG", Color.FromArgb(192, 57, 43), new Point(15, 476), new Size(370, 42), btnClearCart_Click);
            panelRight.Controls.Add(btnClearCart);

            Controls.Add(panelLeft);
            Controls.Add(panelRight);

            ResumeLayout(false);
        }

        // Phím tắt F9 = thanh toán
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F9)
            {
                btnCheckout.PerformClick();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        // ================= GIỎ HÀNG =================
        private decimal Total { get { return _cart.Sum(c => c.TotalPrice); } }

        private static decimal ParseMoney(string text)
        {
            string digits = new string((text ?? "").Where(char.IsDigit).ToArray());
            decimal value;
            return decimal.TryParse(digits, out value) ? value : 0;
        }

        private void RefreshCart()
        {
            _bs.ResetBindings(false);
            decimal total = Total;
            lblTotalAmount.Text = total.ToString("N0") + " đ";

            int points = _customer != null ? (int)(total / 10000m) : 0;
            lblPoints.Text = _customer != null
                ? "Điểm tích lũy đơn này: " + points + " (1 điểm / 10.000đ)"
                : "Khách vãng lai: không tích điểm";
            CalculateChange();
        }

        private void CalculateChange()
        {
            decimal total = Total;
            decimal cash = ParseMoney(txtCashReceived.Text);

            if (string.IsNullOrWhiteSpace(txtCashReceived.Text))
            {
                lblChange.Text = "0 đ";
                lblChange.ForeColor = Color.FromArgb(39, 174, 96);
            }
            else if (cash < total)
            {
                lblChange.Text = "Còn thiếu " + (total - cash).ToString("N0") + " đ";
                lblChange.ForeColor = Color.FromArgb(192, 57, 43);
            }
            else
            {
                lblChange.Text = (cash - total).ToString("N0") + " đ";
                lblChange.ForeColor = Color.FromArgb(39, 174, 96);
            }
        }

        private void AddToCart(ProductRow p)
        {
            var existing = _cart.FirstOrDefault(c => c.ProductId == p.ProductId);
            int newQty = (existing != null ? existing.Quantity : 0) + 1;

            if (newQty > p.StockQuantity)
            {
                MessageBox.Show("Sản phẩm \"" + p.ProductName + "\" chỉ còn " + p.StockQuantity + " trong kho!",
                    "Hết hàng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (existing != null)
            {
                existing.Quantity = newQty;
                existing.StockQuantity = p.StockQuantity;
            }
            else
            {
                _cart.Add(new CartItem
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    UnitPrice = p.Price,
                    Quantity = 1,
                    StockQuantity = p.StockQuantity
                });
            }
            RefreshCart();
        }

        private CartItem SelectedItem()
        {
            return dgvCart.CurrentRow != null ? dgvCart.CurrentRow.DataBoundItem as CartItem : null;
        }

        private void btnInc_Click(object sender, EventArgs e)
        {
            var item = SelectedItem();
            if (item == null) return;
            if (item.Quantity + 1 > item.StockQuantity)
            {
                MessageBox.Show("Chỉ còn " + item.StockQuantity + " sản phẩm trong kho!", "Hết hàng",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            item.Quantity++;
            RefreshCart();
        }

        private void btnDec_Click(object sender, EventArgs e)
        {
            var item = SelectedItem();
            if (item == null) return;
            if (item.Quantity > 1) item.Quantity--;
            else _cart.Remove(item);
            RefreshCart();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            var item = SelectedItem();
            if (item == null) return;
            _cart.Remove(item);
            RefreshCart();
        }

        private void ResetSale()
        {
            _cart.Clear();
            _customer = null;
            txtCustomerPhone.Clear();
            txtCashReceived.Clear();
            lblCustomerName.Text = "Khách vãng lai (không tích điểm)";
            lblCustomerName.ForeColor = Color.DimGray;
            RefreshCart();
            txtBarcode.Focus();
        }

        private void btnClearCart_Click(object sender, EventArgs e)
        {
            if (_cart.Count == 0) { ResetSale(); return; }
            var confirm = MessageBox.Show("Hủy toàn bộ giỏ hàng hiện tại?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes) ResetSale();
        }

        // ================= QUÉT MÃ VẠCH =================
        private async void txtBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true;

            string input = txtBarcode.Text.Trim();
            if (input.Length == 0) return;
            txtBarcode.Clear();

            try
            {
                await AddByInputAsync(input);
            }
            catch (Exception ex)
            {
                if (IsDisposed) return;
                MessageBox.Show("Lỗi kết nối máy chủ: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (!IsDisposed) txtBarcode.Focus();
        }

        private async Task AddByInputAsync(string input)
        {
            var client = ApiClient.GetClient();

            // 1) Thử theo mã vạch
            var res = await client.GetAsync("products/barcode/" + Uri.EscapeDataString(input));
            if (IsDisposed) return;
            if (res.IsSuccessStatusCode)
            {
                var p = await res.Content.ReadFromJsonAsync<ProductRow>();
                if (!IsDisposed && p != null) AddToCart(p);
                return;
            }
            if (res.StatusCode != HttpStatusCode.NotFound)
            {
                await ApiClient.CheckAsync(res);
                return;
            }

            // 2) Không có mã vạch này -> thử tìm theo tên
            var res2 = await client.GetAsync("products?keyword=" + Uri.EscapeDataString(input));
            if (IsDisposed) return;
            if (!await ApiClient.CheckAsync(res2)) return;

            var list = await res2.Content.ReadFromJsonAsync<List<ProductRow>>();
            if (IsDisposed) return;

            if (list == null || list.Count == 0)
                MessageBox.Show("Không tìm thấy sản phẩm có mã vạch hoặc tên này!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else if (list.Count == 1)
                AddToCart(list[0]);
            else
                MessageBox.Show("Có " + list.Count + " sản phẩm khớp, hãy nhập rõ tên hơn hoặc dùng mã vạch.",
                    "Nhiều kết quả", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ================= KHÁCH HÀNG THÂN THIẾT =================
        private void txtCustomerPhone_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnFindCustomer_Click(null, null);
            }
        }

        private async void btnFindCustomer_Click(object sender, EventArgs e)
        {
            string phone = txtCustomerPhone.Text.Trim();
            if (phone.Length == 0)
            {
                _customer = null;
                lblCustomerName.Text = "Khách vãng lai (không tích điểm)";
                lblCustomerName.ForeColor = Color.DimGray;
                RefreshCart();
                return;
            }

            try
            {
                var res = await ApiClient.GetClient().GetAsync("customers/search?keyword=" + Uri.EscapeDataString(phone));
                if (IsDisposed) return;
                if (!await ApiClient.CheckAsync(res)) return;

                var list = await res.Content.ReadFromJsonAsync<List<PosCustomer>>();
                if (IsDisposed) return;

                PosCustomer found = null;
                if (list != null)
                {
                    found = list.FirstOrDefault(c => c.PhoneNumber == phone);
                    if (found == null && list.Count == 1) found = list[0];
                }

                if (found == null)
                {
                    _customer = null;
                    lblCustomerName.Text = "Không tìm thấy khách hàng với số này";
                    lblCustomerName.ForeColor = Color.FromArgb(192, 57, 43);
                }
                else
                {
                    _customer = found;
                    lblCustomerName.Text = found.CustomerName + " — Hạng: " + found.MembershipRank +
                                           " — " + found.RewardPoints + " điểm";
                    lblCustomerName.ForeColor = Color.FromArgb(41, 100, 180);
                }
                RefreshCart();
            }
            catch (Exception ex)
            {
                if (IsDisposed) return;
                MessageBox.Show("Lỗi kết nối máy chủ: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ================= THANH TOÁN =================
        private async void btnCheckout_Click(object sender, EventArgs e)
        {
            if (_cart.Count == 0)
            {
                MessageBox.Show("Giỏ hàng đang trống!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBarcode.Focus();
                return;
            }

            decimal total = Total;
            decimal cash = ParseMoney(txtCashReceived.Text);
            if (cash < total)
            {
                MessageBox.Show("Tiền khách đưa chưa đủ để thanh toán!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCashReceived.Focus();
                return;
            }

            btnCheckout.Enabled = false;
            try
            {
                var payload = new
                {
                    CustomerId = _customer != null ? (int?)_customer.CustomerId : null,
                    CashReceived = cash,
                    Items = _cart.Select(c => new { c.ProductId, c.Quantity }).ToList()
                };

                var res = await ApiClient.GetClient().PostAsJsonAsync("orders/checkout", payload);
                if (IsDisposed) return;
                if (!await ShowApiResultAsync(res)) return;

                var result = await res.Content.ReadFromJsonAsync<CheckoutResult>();
                if (IsDisposed) return;

                string msg = "Thanh toán thành công! Mã hóa đơn: #" + result.OrderId +
                             "\nTổng tiền: " + result.TotalAmount.ToString("N0") + " đ" +
                             "\nTiền thừa: " + result.ChangeAmount.ToString("N0") + " đ";
                if (_customer != null)
                    msg += "\n\nKhách " + _customer.CustomerName + " được cộng " + result.PointsEarned +
                           " điểm (tổng " + result.CustomerPoints + " điểm, hạng " + result.MembershipRank + ").";

                MessageBox.Show(msg, "Thanh toán", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ResetSale();
            }
            catch (Exception ex)
            {
                if (!IsDisposed)
                    MessageBox.Show("Lỗi kết nối máy chủ: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (!IsDisposed) btnCheckout.Enabled = true;
            }
        }

        // Hiện đúng thông báo lỗi của API (VD: "Sản phẩm X chỉ còn 3 trong kho!")
        private async Task<bool> ShowApiResultAsync(HttpResponseMessage res)
        {
            if (res.IsSuccessStatusCode) return true;

            if (res.StatusCode == HttpStatusCode.BadRequest)
            {
                string body = await res.Content.ReadAsStringAsync();
                string msg = "Yêu cầu không hợp lệ.";
                try
                {
                    using (var doc = JsonDocument.Parse(body))
                    {
                        JsonElement m;
                        if (doc.RootElement.TryGetProperty("message", out m)) msg = m.GetString();
                    }
                }
                catch { }
                MessageBox.Show(msg, "Không thể thanh toán", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return await ApiClient.CheckAsync(res);
        }
    }
}