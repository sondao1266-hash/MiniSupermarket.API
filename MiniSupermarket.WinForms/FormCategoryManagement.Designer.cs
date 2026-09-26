namespace MiniSupermarket.WinForms
{
    partial class FormCategoryManagement
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.Label lblID;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblDescription;

        private System.Windows.Forms.Label lblListTitle;
        private System.Windows.Forms.Label lblInfoTitle;

        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.TextBox txtID;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txtDescription;

        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnReload;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnDelete;

        private System.Windows.Forms.DataGridView dataGridView1;

        private System.Windows.Forms.DataGridViewTextBoxColumn colID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDescription;

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();

            this.components = new System.ComponentModel.Container();

            this.lblSearch = new System.Windows.Forms.Label();
            this.lblID = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();

            this.lblListTitle = new System.Windows.Forms.Label();
            this.lblInfoTitle = new System.Windows.Forms.Label();

            this.txtSearch = new System.Windows.Forms.TextBox();
            this.txtID = new System.Windows.Forms.TextBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtDescription = new System.Windows.Forms.TextBox();

            this.btnSearch = new System.Windows.Forms.Button();
            this.btnReload = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();

            this.dataGridView1 = new System.Windows.Forms.DataGridView();

            this.colID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();

            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();

            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();

            // =========================================================
            // FORM
            // =========================================================

            this.AutoScaleDimensions =
                new System.Drawing.SizeF(7F, 15F);

            this.AutoScaleMode =
                System.Windows.Forms.AutoScaleMode.Font;

            this.BackColor =
                System.Drawing.Color.FromArgb(245, 243, 255);

            this.ClientSize =
                new System.Drawing.Size(1000, 620);

            this.MinimumSize =
                new System.Drawing.Size(1000, 620);

            this.StartPosition =
                System.Windows.Forms.FormStartPosition.CenterScreen;

            this.Name =
                "FormCategoryManagement";

            this.Text =
                "Quản lý Nhóm hàng";

            // =========================================================
            // SEARCH LABEL
            // =========================================================

            this.lblSearch.AutoSize = true;

            this.lblSearch.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    10F,
                    System.Drawing.FontStyle.Bold);

            this.lblSearch.ForeColor =
                System.Drawing.Color.FromArgb(76, 29, 149);

            this.lblSearch.Location =
                new System.Drawing.Point(30, 28);

            this.lblSearch.Name =
                "lblSearch";

            this.lblSearch.Text =
                "Tìm kiếm nhóm hàng";

            // =========================================================
            // SEARCH TEXTBOX
            // =========================================================

            this.txtSearch.BackColor =
                System.Drawing.Color.White;

            this.txtSearch.BorderStyle =
                System.Windows.Forms.BorderStyle.FixedSingle;

            this.txtSearch.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    10F);

            this.txtSearch.ForeColor =
                System.Drawing.Color.FromArgb(75, 85, 99);

            this.txtSearch.Location =
                new System.Drawing.Point(30, 55);

            this.txtSearch.Name =
                "txtSearch";

            this.txtSearch.Size =
                new System.Drawing.Size(390, 25);

            this.txtSearch.TabIndex = 0;

            this.txtSearch.Enter +=
                new System.EventHandler(
                    this.txtSearch_Enter);

            this.txtSearch.Leave +=
                new System.EventHandler(
                    this.txtSearch_Leave);

            // =========================================================
            // SEARCH BUTTON
            // =========================================================

            this.btnSearch.BackColor =
                System.Drawing.Color.FromArgb(124, 58, 237);

            this.btnSearch.FlatAppearance.BorderSize = 0;

            this.btnSearch.FlatStyle =
                System.Windows.Forms.FlatStyle.Flat;

            this.btnSearch.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    9F,
                    System.Drawing.FontStyle.Bold);

            this.btnSearch.ForeColor =
                System.Drawing.Color.White;

            this.btnSearch.Location =
                new System.Drawing.Point(435, 53);

            this.btnSearch.Name =
                "btnSearch";

            this.btnSearch.Size =
                new System.Drawing.Size(100, 30);

            this.btnSearch.TabIndex = 1;

            this.btnSearch.Text =
                "Tìm kiếm";

            this.btnSearch.UseVisualStyleBackColor = false;

            this.btnSearch.Cursor =
                System.Windows.Forms.Cursors.Hand;

            this.btnSearch.Click +=
                new System.EventHandler(
                    this.btnSearch_Click);

            // =========================================================
            // RELOAD BUTTON
            // =========================================================

            this.btnReload.BackColor =
                System.Drawing.Color.FromArgb(139, 92, 246);

            this.btnReload.FlatAppearance.BorderSize = 0;

            this.btnReload.FlatStyle =
                System.Windows.Forms.FlatStyle.Flat;

            this.btnReload.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    9F,
                    System.Drawing.FontStyle.Bold);

            this.btnReload.ForeColor =
                System.Drawing.Color.White;

            this.btnReload.Location =
                new System.Drawing.Point(545, 53);

            this.btnReload.Name =
                "btnReload";

            this.btnReload.Size =
                new System.Drawing.Size(90, 30);

            this.btnReload.TabIndex = 2;

            this.btnReload.Text =
                "Tải lại";

            this.btnReload.UseVisualStyleBackColor = false;

            this.btnReload.Cursor =
                System.Windows.Forms.Cursors.Hand;

            this.btnReload.Click +=
                new System.EventHandler(
                    this.btnReload_Click);

            // =========================================================
            // LIST TITLE
            // =========================================================

            this.lblListTitle.AutoSize = true;

            this.lblListTitle.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    12F,
                    System.Drawing.FontStyle.Bold);

            this.lblListTitle.ForeColor =
                System.Drawing.Color.FromArgb(91, 33, 182);

            this.lblListTitle.Location =
                new System.Drawing.Point(30, 105);

            this.lblListTitle.Name =
                "lblListTitle";

            this.lblListTitle.Text =
                "DANH SÁCH NHÓM HÀNG";

            // =========================================================
            // DATA GRID
            // =========================================================

            this.dataGridView1.AllowUserToAddRows = false;

            this.dataGridView1.AllowUserToDeleteRows = false;

            this.dataGridView1.AllowUserToResizeRows = false;

            this.dataGridView1.AutoGenerateColumns = false;

            this.dataGridView1.BackgroundColor =
                System.Drawing.Color.White;

            this.dataGridView1.BorderStyle =
                System.Windows.Forms.BorderStyle.None;

            this.dataGridView1.CellBorderStyle =
                System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;

            this.dataGridView1.ColumnHeadersBorderStyle =
                System.Windows.Forms.DataGridViewHeaderBorderStyle.None;

            dataGridViewCellStyle1.BackColor =
                System.Drawing.Color.FromArgb(109, 40, 217);

            dataGridViewCellStyle1.ForeColor =
                System.Drawing.Color.White;

            dataGridViewCellStyle1.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    9F,
                    System.Drawing.FontStyle.Bold);

            dataGridViewCellStyle1.Alignment =
                System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;

            dataGridViewCellStyle1.SelectionBackColor =
                System.Drawing.Color.FromArgb(109, 40, 217);

            dataGridViewCellStyle1.SelectionForeColor =
                System.Drawing.Color.White;

            this.dataGridView1.ColumnHeadersDefaultCellStyle =
                dataGridViewCellStyle1;

            this.dataGridView1.ColumnHeadersHeight = 40;

            this.dataGridView1.ColumnHeadersHeightSizeMode =
                System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            dataGridViewCellStyle2.BackColor =
                System.Drawing.Color.White;

            dataGridViewCellStyle2.ForeColor =
                System.Drawing.Color.FromArgb(55, 65, 81);

            dataGridViewCellStyle2.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    9F);

            dataGridViewCellStyle2.SelectionBackColor =
                System.Drawing.Color.FromArgb(237, 233, 254);

            dataGridViewCellStyle2.SelectionForeColor =
                System.Drawing.Color.FromArgb(76, 29, 149);

            dataGridViewCellStyle2.Padding =
                new System.Windows.Forms.Padding(
                    5,
                    0,
                    5,
                    0);

            this.dataGridView1.DefaultCellStyle =
                dataGridViewCellStyle2;

            this.dataGridView1.EnableHeadersVisualStyles = false;

            this.dataGridView1.GridColor =
                System.Drawing.Color.FromArgb(229, 231, 235);

            this.dataGridView1.Location =
                new System.Drawing.Point(30, 135);

            this.dataGridView1.MultiSelect = false;

            this.dataGridView1.Name =
                "dataGridView1";

            this.dataGridView1.ReadOnly = true;

            this.dataGridView1.RowHeadersVisible = false;

            this.dataGridView1.RowTemplate.Height = 38;

            this.dataGridView1.SelectionMode =
                System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;

            this.dataGridView1.Size =
                new System.Drawing.Size(575, 390);

            this.dataGridView1.TabIndex = 3;

            this.dataGridView1.Columns.AddRange(
                new System.Windows.Forms.DataGridViewColumn[]
                {
                    this.colID,
                    this.colName,
                    this.colDescription
                });

            this.dataGridView1.CellClick +=
                new System.Windows.Forms.DataGridViewCellEventHandler(
                    this.dataGridView1_CellClick);

            // =========================================================
            // COLUMN ID
            // =========================================================

            this.colID.DataPropertyName =
                "ID";

            this.colID.HeaderText =
                "Mã ID";

            this.colID.Name =
                "colID";

            this.colID.ReadOnly =
                true;

            this.colID.Width =
                70;

            // =========================================================
            // COLUMN NAME
            // =========================================================

            this.colName.DataPropertyName =
                "Name";

            this.colName.HeaderText =
                "Tên nhóm hàng";

            this.colName.Name =
                "colName";

            this.colName.ReadOnly =
                true;

            this.colName.Width =
                190;

            // =========================================================
            // COLUMN DESCRIPTION
            // =========================================================

            this.colDescription.DataPropertyName =
                "Description";

            this.colDescription.HeaderText =
                "Mô tả";

            this.colDescription.Name =
                "colDescription";

            this.colDescription.ReadOnly =
                true;

            this.colDescription.AutoSizeMode =
                System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;

            // =========================================================
            // INFO TITLE
            // =========================================================

            this.lblInfoTitle.AutoSize = true;

            this.lblInfoTitle.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    12F,
                    System.Drawing.FontStyle.Bold);

            this.lblInfoTitle.ForeColor =
                System.Drawing.Color.FromArgb(91, 33, 182);

            this.lblInfoTitle.Location =
                new System.Drawing.Point(635, 105);

            this.lblInfoTitle.Name =
                "lblInfoTitle";

            this.lblInfoTitle.Text =
                "THÔNG TIN NHÓM HÀNG";

            // =========================================================
            // ID LABEL
            // =========================================================

            this.lblID.AutoSize = true;

            this.lblID.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    9F,
                    System.Drawing.FontStyle.Bold);

            this.lblID.ForeColor =
                System.Drawing.Color.FromArgb(76, 29, 149);

            this.lblID.Location =
                new System.Drawing.Point(635, 140);

            this.lblID.Name =
                "lblID";

            this.lblID.Text =
                "Mã ID";

            // =========================================================
            // ID TEXTBOX
            // =========================================================

            this.txtID.BackColor =
                System.Drawing.Color.FromArgb(237, 233, 254);

            this.txtID.BorderStyle =
                System.Windows.Forms.BorderStyle.FixedSingle;

            this.txtID.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    10F);

            this.txtID.ForeColor =
                System.Drawing.Color.FromArgb(107, 33, 168);

            this.txtID.Location =
                new System.Drawing.Point(635, 163);

            this.txtID.Name =
                "txtID";

            this.txtID.ReadOnly =
                true;

            this.txtID.Size =
                new System.Drawing.Size(315, 25);

            this.txtID.TabIndex = 4;

            // =========================================================
            // NAME LABEL
            // =========================================================

            this.lblName.AutoSize = true;

            this.lblName.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    9F,
                    System.Drawing.FontStyle.Bold);

            this.lblName.ForeColor =
                System.Drawing.Color.FromArgb(76, 29, 149);

            this.lblName.Location =
                new System.Drawing.Point(635, 205);

            this.lblName.Name =
                "lblName";

            this.lblName.Text =
                "Tên nhóm hàng";

            // =========================================================
            // NAME TEXTBOX
            // =========================================================

            this.txtName.BackColor =
                System.Drawing.Color.White;

            this.txtName.BorderStyle =
                System.Windows.Forms.BorderStyle.FixedSingle;

            this.txtName.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    10F);

            this.txtName.ForeColor =
                System.Drawing.Color.FromArgb(31, 41, 55);

            this.txtName.Location =
                new System.Drawing.Point(635, 228);

            this.txtName.Name =
                "txtName";

            this.txtName.Size =
                new System.Drawing.Size(315, 25);

            this.txtName.TabIndex = 5;

            // =========================================================
            // DESCRIPTION LABEL
            // =========================================================

            this.lblDescription.AutoSize = true;

            this.lblDescription.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    9F,
                    System.Drawing.FontStyle.Bold);

            this.lblDescription.ForeColor =
                System.Drawing.Color.FromArgb(76, 29, 149);

            this.lblDescription.Location =
                new System.Drawing.Point(635, 270);

            this.lblDescription.Name =
                "lblDescription";

            this.lblDescription.Text =
                "Mô tả";

            // =========================================================
            // DESCRIPTION TEXTBOX
            // =========================================================

            this.txtDescription.BackColor =
                System.Drawing.Color.White;

            this.txtDescription.BorderStyle =
                System.Windows.Forms.BorderStyle.FixedSingle;

            this.txtDescription.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    10F);

            this.txtDescription.ForeColor =
                System.Drawing.Color.FromArgb(31, 41, 55);

            this.txtDescription.Location =
                new System.Drawing.Point(635, 293);

            this.txtDescription.Multiline = true;

            this.txtDescription.Name =
                "txtDescription";

            this.txtDescription.ScrollBars =
                System.Windows.Forms.ScrollBars.Vertical;

            this.txtDescription.Size =
                new System.Drawing.Size(315, 110);

            this.txtDescription.TabIndex = 6;

            // =========================================================
            // ADD BUTTON
            // =========================================================

            this.btnAdd.BackColor =
                System.Drawing.Color.FromArgb(139, 92, 246);

            this.btnAdd.FlatAppearance.BorderSize = 0;

            this.btnAdd.FlatStyle =
                System.Windows.Forms.FlatStyle.Flat;

            this.btnAdd.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    9F,
                    System.Drawing.FontStyle.Bold);

            this.btnAdd.ForeColor =
                System.Drawing.Color.White;

            this.btnAdd.Location =
                new System.Drawing.Point(635, 430);

            this.btnAdd.Name =
                "btnAdd";

            this.btnAdd.Size =
                new System.Drawing.Size(95, 35);

            this.btnAdd.TabIndex = 7;

            this.btnAdd.Text =
                "Thêm mới";

            this.btnAdd.UseVisualStyleBackColor = false;

            this.btnAdd.Cursor =
                System.Windows.Forms.Cursors.Hand;

            this.btnAdd.Click +=
                new System.EventHandler(
                    this.btnAdd_Click);

            // =========================================================
            // UPDATE BUTTON
            // =========================================================

            this.btnUpdate.BackColor =
                System.Drawing.Color.FromArgb(236, 72, 153);

            this.btnUpdate.FlatAppearance.BorderSize = 0;

            this.btnUpdate.FlatStyle =
                System.Windows.Forms.FlatStyle.Flat;

            this.btnUpdate.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    9F,
                    System.Drawing.FontStyle.Bold);

            this.btnUpdate.ForeColor =
                System.Drawing.Color.White;

            this.btnUpdate.Location =
                new System.Drawing.Point(740, 430);

            this.btnUpdate.Name =
                "btnUpdate";

            this.btnUpdate.Size =
                new System.Drawing.Size(100, 35);

            this.btnUpdate.TabIndex = 8;

            this.btnUpdate.Text =
                "Cập nhật";

            this.btnUpdate.UseVisualStyleBackColor = false;

            this.btnUpdate.Cursor =
                System.Windows.Forms.Cursors.Hand;

            this.btnUpdate.Click +=
                new System.EventHandler(
                    this.btnUpdate_Click);

            // =========================================================
            // DELETE BUTTON
            // =========================================================

            this.btnDelete.BackColor =
                System.Drawing.Color.FromArgb(239, 68, 68);

            this.btnDelete.FlatAppearance.BorderSize = 0;

            this.btnDelete.FlatStyle =
                System.Windows.Forms.FlatStyle.Flat;

            this.btnDelete.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    9F,
                    System.Drawing.FontStyle.Bold);

            this.btnDelete.ForeColor =
                System.Drawing.Color.White;

            this.btnDelete.Location =
                new System.Drawing.Point(850, 430);

            this.btnDelete.Name =
                "btnDelete";

            this.btnDelete.Size =
                new System.Drawing.Size(100, 35);

            this.btnDelete.TabIndex = 9;

            this.btnDelete.Text =
                "Xóa";

            this.btnDelete.UseVisualStyleBackColor = false;

            this.btnDelete.Cursor =
                System.Windows.Forms.Cursors.Hand;

            this.btnDelete.Click +=
                new System.EventHandler(
                    this.btnDelete_Click);

            // =========================================================
            // STATUS STRIP
            // =========================================================

            this.statusStrip1.BackColor =
                System.Drawing.Color.FromArgb(237, 233, 254);

            this.statusStrip1.Items.AddRange(
                new System.Windows.Forms.ToolStripItem[]
                {
                    this.lblStatus
                });

            this.statusStrip1.Location =
                new System.Drawing.Point(0, 598);

            this.statusStrip1.Name =
                "statusStrip1";

            this.statusStrip1.Size =
                new System.Drawing.Size(1000, 22);

            this.statusStrip1.TabIndex = 10;

            // =========================================================
            // STATUS
            // =========================================================

            this.lblStatus.ForeColor =
                System.Drawing.Color.FromArgb(91, 33, 182);

            this.lblStatus.Name =
                "lblStatus";

            this.lblStatus.Size =
                new System.Drawing.Size(39, 17);

            this.lblStatus.Text =
                "Ready";

            // =========================================================
            // ADD CONTROLS
            // =========================================================

            this.Controls.Add(this.lblSearch);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.btnReload);

            this.Controls.Add(this.lblListTitle);
            this.Controls.Add(this.dataGridView1);

            this.Controls.Add(this.lblInfoTitle);

            this.Controls.Add(this.lblID);
            this.Controls.Add(this.txtID);

            this.Controls.Add(this.lblName);
            this.Controls.Add(this.txtName);

            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.txtDescription);

            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.btnDelete);

            this.Controls.Add(this.statusStrip1);

            // =========================================================
            // LOAD EVENT
            // =========================================================

            this.Load +=
                new System.EventHandler(
                    this.FormCategoryManagement_Load_1);

            ((System.ComponentModel.ISupportInitialize)
                (this.dataGridView1)).EndInit();

            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();

            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}