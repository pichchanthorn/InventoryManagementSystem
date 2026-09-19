namespace InventoryManagementSystem.UI
{
    partial class ucDashboard
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlTop = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblLastRefreshed = new System.Windows.Forms.Label();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.tlpCards = new System.Windows.Forms.TableLayoutPanel();
            this.tlpLists = new System.Windows.Forms.TableLayoutPanel();
            this.pnlLowStock = new System.Windows.Forms.Panel();
            this.dgvLowStock = new System.Windows.Forms.DataGridView();
            this.lblLowStockEmpty = new System.Windows.Forms.Label();
            this.lblLowStockHeader = new System.Windows.Forms.Label();
            this.pnlRecent = new System.Windows.Forms.Panel();
            this.dgvRecent = new System.Windows.Forms.DataGridView();
            this.lblRecentEmpty = new System.Windows.Forms.Label();
            this.lblRecentHeader = new System.Windows.Forms.Label();
            this.pnlTop.SuspendLayout();
            this.tlpLists.SuspendLayout();
            this.pnlLowStock.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLowStock)).BeginInit();
            this.pnlRecent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecent)).BeginInit();
            this.SuspendLayout();
            //
            // pnlTop
            //
            this.pnlTop.Controls.Add(this.lblTitle);
            this.pnlTop.Controls.Add(this.lblLastRefreshed);
            this.pnlTop.Controls.Add(this.btnRefresh);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(900, 36);
            this.pnlTop.TabIndex = 0;
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(0, 4);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(101, 25);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Dashboard";
            //
            // lblLastRefreshed
            //
            this.lblLastRefreshed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLastRefreshed.ForeColor = System.Drawing.Color.DimGray;
            this.lblLastRefreshed.Location = new System.Drawing.Point(480, 11);
            this.lblLastRefreshed.Name = "lblLastRefreshed";
            this.lblLastRefreshed.Size = new System.Drawing.Size(325, 15);
            this.lblLastRefreshed.TabIndex = 1;
            this.lblLastRefreshed.Text = "Last refreshed: -";
            this.lblLastRefreshed.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // btnRefresh
            //
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Location = new System.Drawing.Point(815, 5);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(85, 26);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            //
            // tlpCards
            //
            this.tlpCards.ColumnCount = 4;
            this.tlpCards.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpCards.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpCards.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpCards.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpCards.Dock = System.Windows.Forms.DockStyle.Top;
            this.tlpCards.Location = new System.Drawing.Point(0, 36);
            this.tlpCards.Name = "tlpCards";
            this.tlpCards.RowCount = 2;
            this.tlpCards.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpCards.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpCards.Size = new System.Drawing.Size(900, 116);
            this.tlpCards.TabIndex = 1;
            //
            // tlpLists
            //
            this.tlpLists.ColumnCount = 2;
            this.tlpLists.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 42F));
            this.tlpLists.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 58F));
            this.tlpLists.Controls.Add(this.pnlLowStock, 0, 0);
            this.tlpLists.Controls.Add(this.pnlRecent, 1, 0);
            this.tlpLists.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpLists.Location = new System.Drawing.Point(0, 152);
            this.tlpLists.Name = "tlpLists";
            this.tlpLists.RowCount = 1;
            this.tlpLists.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpLists.Size = new System.Drawing.Size(900, 300);
            this.tlpLists.TabIndex = 2;
            //
            // pnlLowStock
            //
            this.pnlLowStock.Controls.Add(this.dgvLowStock);
            this.pnlLowStock.Controls.Add(this.lblLowStockEmpty);
            this.pnlLowStock.Controls.Add(this.lblLowStockHeader);
            this.pnlLowStock.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlLowStock.Margin = new System.Windows.Forms.Padding(0, 6, 6, 0);
            this.pnlLowStock.Name = "pnlLowStock";
            this.pnlLowStock.TabIndex = 0;
            //
            // lblLowStockHeader
            //
            this.lblLowStockHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblLowStockHeader.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblLowStockHeader.Name = "lblLowStockHeader";
            this.lblLowStockHeader.Size = new System.Drawing.Size(300, 24);
            this.lblLowStockHeader.TabIndex = 0;
            this.lblLowStockHeader.Text = "Low Stock Products";
            this.lblLowStockHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblLowStockEmpty
            //
            this.lblLowStockEmpty.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblLowStockEmpty.ForeColor = System.Drawing.Color.DimGray;
            this.lblLowStockEmpty.Name = "lblLowStockEmpty";
            this.lblLowStockEmpty.TabIndex = 1;
            this.lblLowStockEmpty.Text = "No products are at or below their reorder level.";
            this.lblLowStockEmpty.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblLowStockEmpty.Visible = false;
            //
            // dgvLowStock
            //
            this.dgvLowStock.AllowUserToAddRows = false;
            this.dgvLowStock.AllowUserToDeleteRows = false;
            this.dgvLowStock.AllowUserToResizeRows = false;
            this.dgvLowStock.AutoGenerateColumns = false;
            this.dgvLowStock.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvLowStock.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLowStock.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvLowStock.MultiSelect = false;
            this.dgvLowStock.Name = "dgvLowStock";
            this.dgvLowStock.ReadOnly = true;
            this.dgvLowStock.RowHeadersVisible = false;
            this.dgvLowStock.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLowStock.TabIndex = 2;
            //
            // pnlRecent
            //
            this.pnlRecent.Controls.Add(this.dgvRecent);
            this.pnlRecent.Controls.Add(this.lblRecentEmpty);
            this.pnlRecent.Controls.Add(this.lblRecentHeader);
            this.pnlRecent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRecent.Margin = new System.Windows.Forms.Padding(6, 6, 0, 0);
            this.pnlRecent.Name = "pnlRecent";
            this.pnlRecent.TabIndex = 1;
            //
            // lblRecentHeader
            //
            this.lblRecentHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblRecentHeader.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblRecentHeader.Name = "lblRecentHeader";
            this.lblRecentHeader.Size = new System.Drawing.Size(300, 24);
            this.lblRecentHeader.TabIndex = 0;
            this.lblRecentHeader.Text = "Recent Activity";
            this.lblRecentHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblRecentEmpty
            //
            this.lblRecentEmpty.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblRecentEmpty.ForeColor = System.Drawing.Color.DimGray;
            this.lblRecentEmpty.Name = "lblRecentEmpty";
            this.lblRecentEmpty.TabIndex = 1;
            this.lblRecentEmpty.Text = "No recent activity yet.";
            this.lblRecentEmpty.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblRecentEmpty.Visible = false;
            //
            // dgvRecent
            //
            this.dgvRecent.AllowUserToAddRows = false;
            this.dgvRecent.AllowUserToDeleteRows = false;
            this.dgvRecent.AllowUserToResizeRows = false;
            this.dgvRecent.AutoGenerateColumns = false;
            this.dgvRecent.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvRecent.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRecent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvRecent.MultiSelect = false;
            this.dgvRecent.Name = "dgvRecent";
            this.dgvRecent.ReadOnly = true;
            this.dgvRecent.RowHeadersVisible = false;
            this.dgvRecent.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvRecent.TabIndex = 2;
            //
            // ucDashboard
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tlpLists);
            this.Controls.Add(this.tlpCards);
            this.Controls.Add(this.pnlTop);
            this.Name = "ucDashboard";
            this.Size = new System.Drawing.Size(900, 452);
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.tlpLists.ResumeLayout(false);
            this.pnlLowStock.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLowStock)).EndInit();
            this.pnlRecent.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecent)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblLastRefreshed;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.TableLayoutPanel tlpCards;
        private System.Windows.Forms.TableLayoutPanel tlpLists;
        private System.Windows.Forms.Panel pnlLowStock;
        private System.Windows.Forms.DataGridView dgvLowStock;
        private System.Windows.Forms.Label lblLowStockEmpty;
        private System.Windows.Forms.Label lblLowStockHeader;
        private System.Windows.Forms.Panel pnlRecent;
        private System.Windows.Forms.DataGridView dgvRecent;
        private System.Windows.Forms.Label lblRecentEmpty;
        private System.Windows.Forms.Label lblRecentHeader;
    }
}
