using System;
using System.Drawing;
using System.Net.Http.Json;
using System.Windows.Forms;

namespace MiniSupermarket.WinForms
{
    public class DailyReportDto
    {
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public string BestSeller { get; set; }
    }

    public class FormQuickReport : Form
    {
        private DateTimePicker dtpDate;
        private Button btnRunReport;
        private Label lblTotalOrders, lblTotalRevenue, lblBestSeller;

        public FormQuickReport()
        {
            BuildUI();
        }

        // Tạo một thẻ tóm tắt: tiêu đề nhỏ + số liệu lớn
        private Panel MakeCard(string title, Color color, out Label valueLabel)
        {
            var card = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = color,
                Margin = new Padding(10)
            };
            var lblTitle = new Label
            {
                Text = title,
                Dock = DockStyle.Top,
                Height = 40,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11f),
                Padding = new Padding(15, 12, 0, 0)
            };
            valueLabel = new Label
            {
                Text = "—",
                Dock = DockStyle.Fill,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 20f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                UseMnemonic = false
            };
            card.Controls.Add(valueLabel);
            card.Controls.Add(lblTitle);
            return card;
        }

        private void BuildUI()
        {
            SuspendLayout();
            Text = "Báo cáo Doanh thu";
            BackColor = Color.FromArgb(244, 245, 247);
            ClientSize = new Size(1000, 600);

            // ---- Thanh chọn ngày ----
            var panelTop = new Panel { Dock = DockStyle.Top, Height = 65, BackColor = Color.White };
            panelTop.Controls.Add(new Label
            {
                Text = "Chọn ngày xem báo cáo:",
                Location = new Point(20, 23),
                AutoSize = true,
                Font = new Font("Segoe UI", 10f)
            });
            dtpDate = new DateTimePicker
            {
                Location = new Point(210, 19),
                Width = 160,
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today
            };
            panelTop.Controls.Add(dtpDate);

            btnRunReport = new Button
            {
                Text = "📊 Xem báo cáo",
                Location = new Point(390, 16),
                Size = new Size(160, 34),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(41, 100, 180),
                ForeColor = Color.White,
                Font = new Font("Segoe UI Emoji", 10f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRunReport.FlatAppearance.BorderSize = 0;
            btnRunReport.Click += btnRunReport_Click;
            panelTop.Controls.Add(btnRunReport);

            // ---- 3 thẻ tóm tắt ----
            var table = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 190,
                ColumnCount = 3,
                RowCount = 1,
                Padding = new Padding(10, 15, 10, 0)
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34f));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            table.Controls.Add(MakeCard("Tổng số hóa đơn", Color.FromArgb(41, 100, 180), out lblTotalOrders), 0, 0);
            table.Controls.Add(MakeCard("Tổng doanh thu", Color.FromArgb(39, 174, 96), out lblTotalRevenue), 1, 0);
            table.Controls.Add(MakeCard("Mặt hàng bán chạy nhất", Color.FromArgb(230, 126, 34), out lblBestSeller), 2, 0);
            lblBestSeller.Font = new Font("Segoe UI", 13f, FontStyle.Bold);

            var lblNote = new Label
            {
                Dock = DockStyle.Top,
                Height = 40,
                ForeColor = Color.Gray,
                Padding = new Padding(20, 12, 0, 0)
            };

            Controls.Add(lblNote);
            Controls.Add(table);
            Controls.Add(panelTop);

            ResumeLayout(false);
        }

        private async void btnRunReport_Click(object sender, EventArgs e)
        {
            // Lớp bảo vệ cấp Action: Admin và Cashier được xem
            bool allowed = string.Equals(SessionManager.CurrentRole, "Admin", StringComparison.OrdinalIgnoreCase)
              || string.Equals(SessionManager.CurrentRole, "Cashier", StringComparison.OrdinalIgnoreCase);
            if (!allowed)
            {
                MessageBox.Show("Bạn không có quyền xem dữ liệu tài chính của siêu thị!", "Từ chối",
                    MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            try
            {
                btnRunReport.Enabled = false;
                string date = dtpDate.Value.ToString("yyyy-MM-dd");
                var res = await ApiClient.GetClient().GetAsync("reports/daily?date=" + date);
                if (!await ApiClient.CheckAsync(res)) return;

                var report = await res.Content.ReadFromJsonAsync<DailyReportDto>();
                lblTotalOrders.Text = report.TotalOrders.ToString("N0");
                lblTotalRevenue.Text = report.TotalRevenue.ToString("N0") + " đ";
                lblBestSeller.Text = report.BestSeller;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối Server: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnRunReport.Enabled = true;
            }
        }
    }
}