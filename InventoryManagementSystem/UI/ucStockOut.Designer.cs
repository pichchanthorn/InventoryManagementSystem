namespace InventoryManagementSystem.UI
{
    partial class ucStockOut
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlForm = new System.Windows.Forms.Panel();
            this.lblHeader = new System.Windows.Forms.Label();
            this.lblProduct = new System.Windows.Forms.Label();
            this.cmbProduct = new System.Windows.Forms.ComboBox();
            this.lblAvailableStock = new System.Windows.Forms.Label();
            this.txtAvailableStock = new System.Windows.Forms.TextBox();
            this.lblCustomer = new System.Windows.Forms.Label();
            this.cmbCustomer = new System.Windows.Forms.ComboBox();
            this.lblQuantity = new System.Windows.Forms.Label();
            this.txtQuantity = new System.Windows.Forms.TextBox();
            this.lblUnitPrice = new System.Windows.Forms.Label();
            this.txtUnitPrice = new System.Windows.Forms.TextBox();
            this.lblTotalPrice = new System.Windows.Forms.Label();
            this.txtTotalPrice = new System.Windows.Forms.TextBox();
            this.lblNotes = new System.Windows.Forms.Label();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.pnlFilter = new System.Windows.Forms.Panel();
            this.lblFilterProduct = new System.Windows.Forms.Label();
            this.cmbFilterProduct = new System.Windows.Forms.ComboBox();
            this.lblFilterCustomer = new System.Windows.Forms.Label();
            this.cmbFilterCustomer = new System.Windows.Forms.ComboBox();
            this.chkDateFilter = new System.Windows.Forms.CheckBox();
            this.lblDateFrom = new System.Windows.Forms.Label();
            this.dtpFrom = new System.Windows.Forms.DateTimePicker();
            this.lblDateTo = new System.Windows.Forms.Label();
            this.dtpTo = new System.Windows.Forms.DateTimePicker();
            this.lblSearch = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.dgvStockOut = new System.Windows.Forms.DataGridView();
            this.colStockOutID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProductName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCustomerName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colQuantity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUnitPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTotalPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDateOut = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNotes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlForm.SuspendLayout();
            this.pnlFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStockOut)).BeginInit();
            this.SuspendLayout();
            //
            // pnlForm
            //
            this.pnlForm.Controls.Add(this.lblHeader);
            this.pnlForm.Controls.Add(this.lblProduct);
            this.pnlForm.Controls.Add(this.cmbProduct);
            this.pnlForm.Controls.Add(this.lblAvailableStock);
            this.pnlForm.Controls.Add(this.txtAvailableStock);
            this.pnlForm.Controls.Add(this.lblCustomer);
            this.pnlForm.Controls.Add(this.cmbCustomer);
            this.pnlForm.Controls.Add(this.lblQuantity);
            this.pnlForm.Controls.Add(this.txtQuantity);
            this.pnlForm.Controls.Add(this.lblUnitPrice);
            this.pnlForm.Controls.Add(this.txtUnitPrice);
            this.pnlForm.Controls.Add(this.lblTotalPrice);
            this.pnlForm.Controls.Add(this.txtTotalPrice);
            this.pnlForm.Controls.Add(this.lblNotes);
            this.pnlForm.Controls.Add(this.txtNotes);
            this.pnlForm.Controls.Add(this.btnAdd);
            this.pnlForm.Controls.Add(this.btnClear);
            this.pnlForm.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlForm.Location = new System.Drawing.Point(0, 0);
            this.pnlForm.Name = "pnlForm";
            this.pnlForm.Padding = new System.Windows.Forms.Padding(20);
            this.pnlForm.Size = new System.Drawing.Size(900, 285);
            this.pnlForm.TabIndex = 0;
            //
            // lblHeader
            //
            this.lblHeader.AutoSize = true;
            this.lblHeader.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.lblHeader.Location = new System.Drawing.Point(20, 15);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(105, 25);
            this.lblHeader.TabIndex = 0;
            this.lblHeader.Text = "Stock Out";
            //
            // lblProduct
            //
            this.lblProduct.AutoSize = true;
            this.lblProduct.Location = new System.Drawing.Point(20, 60);
            this.lblProduct.Name = "lblProduct";
            this.lblProduct.Size = new System.Drawing.Size(45, 13);
            this.lblProduct.TabIndex = 1;
            this.lblProduct.Text = "Product:";
            //
            // cmbProduct
            //
            this.cmbProduct.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProduct.FormattingEnabled = true;
            this.cmbProduct.Location = new System.Drawing.Point(150, 57);
            this.cmbProduct.Name = "cmbProduct";
            this.cmbProduct.Size = new System.Drawing.Size(300, 21);
            this.cmbProduct.TabIndex = 2;
            this.cmbProduct.SelectedIndexChanged += new System.EventHandler(this.cmbProduct_SelectedIndexChanged);
            //
            // lblAvailableStock
            //
            this.lblAvailableStock.AutoSize = true;
            this.lblAvailableStock.Location = new System.Drawing.Point(480, 60);
            this.lblAvailableStock.Name = "lblAvailableStock";
            this.lblAvailableStock.Size = new System.Drawing.Size(88, 13);
            this.lblAvailableStock.TabIndex = 3;
            this.lblAvailableStock.Text = "Available Stock:";
            //
            // txtAvailableStock
            //
            this.txtAvailableStock.Location = new System.Drawing.Point(600, 57);
            this.txtAvailableStock.Name = "txtAvailableStock";
            this.txtAvailableStock.ReadOnly = true;
            this.txtAvailableStock.Size = new System.Drawing.Size(100, 20);
            this.txtAvailableStock.TabIndex = 4;
            this.txtAvailableStock.TabStop = false;
            //
            // lblCustomer
            //
            this.lblCustomer.AutoSize = true;
            this.lblCustomer.Location = new System.Drawing.Point(20, 95);
            this.lblCustomer.Name = "lblCustomer";
            this.lblCustomer.Size = new System.Drawing.Size(55, 13);
            this.lblCustomer.TabIndex = 5;
            this.lblCustomer.Text = "Customer:";
            //
            // cmbCustomer
            //
            this.cmbCustomer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCustomer.FormattingEnabled = true;
            this.cmbCustomer.Location = new System.Drawing.Point(150, 92);
            this.cmbCustomer.Name = "cmbCustomer";
            this.cmbCustomer.Size = new System.Drawing.Size(300, 21);
            this.cmbCustomer.TabIndex = 6;
            //
            // lblQuantity
            //
            this.lblQuantity.AutoSize = true;
            this.lblQuantity.Location = new System.Drawing.Point(480, 95);
            this.lblQuantity.Name = "lblQuantity";
            this.lblQuantity.Size = new System.Drawing.Size(52, 13);
            this.lblQuantity.TabIndex = 7;
            this.lblQuantity.Text = "Quantity:";
            //
            // txtQuantity
            //
            this.txtQuantity.Location = new System.Drawing.Point(600, 92);
            this.txtQuantity.Name = "txtQuantity";
            this.txtQuantity.Size = new System.Drawing.Size(100, 20);
            this.txtQuantity.TabIndex = 8;
            this.txtQuantity.TextChanged += new System.EventHandler(this.QuantityOrUnitPrice_TextChanged);
            //
            // lblUnitPrice
            //
            this.lblUnitPrice.AutoSize = true;
            this.lblUnitPrice.Location = new System.Drawing.Point(20, 130);
            this.lblUnitPrice.Name = "lblUnitPrice";
            this.lblUnitPrice.Size = new System.Drawing.Size(58, 13);
            this.lblUnitPrice.TabIndex = 9;
            this.lblUnitPrice.Text = "Unit Price (USD):";
            //
            // txtUnitPrice
            //
            this.txtUnitPrice.Location = new System.Drawing.Point(150, 127);
            this.txtUnitPrice.Name = "txtUnitPrice";
            this.txtUnitPrice.Size = new System.Drawing.Size(120, 20);
            this.txtUnitPrice.TabIndex = 10;
            this.txtUnitPrice.TextChanged += new System.EventHandler(this.QuantityOrUnitPrice_TextChanged);
            //
            // lblTotalPrice
            //
            this.lblTotalPrice.AutoSize = true;
            this.lblTotalPrice.Location = new System.Drawing.Point(480, 130);
            this.lblTotalPrice.Name = "lblTotalPrice";
            this.lblTotalPrice.Size = new System.Drawing.Size(102, 13);
            this.lblTotalPrice.TabIndex = 11;
            this.lblTotalPrice.Text = "Total Price (USD):";
            //
            // txtTotalPrice
            //
            this.txtTotalPrice.Location = new System.Drawing.Point(600, 127);
            this.txtTotalPrice.Name = "txtTotalPrice";
            this.txtTotalPrice.ReadOnly = true;
            this.txtTotalPrice.Size = new System.Drawing.Size(120, 20);
            this.txtTotalPrice.TabIndex = 12;
            this.txtTotalPrice.TabStop = false;
            //
            // lblNotes
            //
            this.lblNotes.AutoSize = true;
            this.lblNotes.Location = new System.Drawing.Point(20, 165);
            this.lblNotes.Name = "lblNotes";
            this.lblNotes.Size = new System.Drawing.Size(40, 13);
            this.lblNotes.TabIndex = 13;
            this.lblNotes.Text = "Notes:";
            //
            // txtNotes
            //
            this.txtNotes.Location = new System.Drawing.Point(150, 162);
            this.txtNotes.MaxLength = 255;
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtNotes.Size = new System.Drawing.Size(670, 50);
            this.txtNotes.TabIndex = 14;
            //
            // btnAdd
            //
            this.btnAdd.Location = new System.Drawing.Point(150, 230);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(150, 30);
            this.btnAdd.TabIndex = 15;
            this.btnAdd.Text = "Record Stock Out";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            //
            // btnClear
            //
            this.btnClear.Location = new System.Drawing.Point(310, 230);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(85, 30);
            this.btnClear.TabIndex = 16;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            //
            // pnlFilter
            //
            this.pnlFilter.Controls.Add(this.lblFilterProduct);
            this.pnlFilter.Controls.Add(this.cmbFilterProduct);
            this.pnlFilter.Controls.Add(this.lblFilterCustomer);
            this.pnlFilter.Controls.Add(this.cmbFilterCustomer);
            this.pnlFilter.Controls.Add(this.chkDateFilter);
            this.pnlFilter.Controls.Add(this.lblDateFrom);
            this.pnlFilter.Controls.Add(this.dtpFrom);
            this.pnlFilter.Controls.Add(this.lblDateTo);
            this.pnlFilter.Controls.Add(this.dtpTo);
            this.pnlFilter.Controls.Add(this.lblSearch);
            this.pnlFilter.Controls.Add(this.txtSearch);
            this.pnlFilter.Controls.Add(this.btnSearch);
            this.pnlFilter.Controls.Add(this.btnRefresh);
            this.pnlFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFilter.Location = new System.Drawing.Point(0, 285);
            this.pnlFilter.Name = "pnlFilter";
            this.pnlFilter.Size = new System.Drawing.Size(900, 90);
            this.pnlFilter.TabIndex = 1;
            //
            // lblFilterProduct
            //
            this.lblFilterProduct.AutoSize = true;
            this.lblFilterProduct.Location = new System.Drawing.Point(20, 13);
            this.lblFilterProduct.Name = "lblFilterProduct";
            this.lblFilterProduct.Size = new System.Drawing.Size(45, 13);
            this.lblFilterProduct.TabIndex = 0;
            this.lblFilterProduct.Text = "Product:";
            //
            // cmbFilterProduct
            //
            this.cmbFilterProduct.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFilterProduct.FormattingEnabled = true;
            this.cmbFilterProduct.Location = new System.Drawing.Point(90, 10);
            this.cmbFilterProduct.Name = "cmbFilterProduct";
            this.cmbFilterProduct.Size = new System.Drawing.Size(180, 21);
            this.cmbFilterProduct.TabIndex = 1;
            //
            // lblFilterCustomer
            //
            this.lblFilterCustomer.AutoSize = true;
            this.lblFilterCustomer.Location = new System.Drawing.Point(290, 13);
            this.lblFilterCustomer.Name = "lblFilterCustomer";
            this.lblFilterCustomer.Size = new System.Drawing.Size(58, 13);
            this.lblFilterCustomer.TabIndex = 2;
            this.lblFilterCustomer.Text = "Customer:";
            //
            // cmbFilterCustomer
            //
            this.cmbFilterCustomer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFilterCustomer.FormattingEnabled = true;
            this.cmbFilterCustomer.Location = new System.Drawing.Point(360, 10);
            this.cmbFilterCustomer.Name = "cmbFilterCustomer";
            this.cmbFilterCustomer.Size = new System.Drawing.Size(180, 21);
            this.cmbFilterCustomer.TabIndex = 3;
            //
            // chkDateFilter
            //
            this.chkDateFilter.AutoSize = true;
            this.chkDateFilter.Location = new System.Drawing.Point(560, 12);
            this.chkDateFilter.Name = "chkDateFilter";
            this.chkDateFilter.Size = new System.Drawing.Size(93, 17);
            this.chkDateFilter.TabIndex = 4;
            this.chkDateFilter.Text = "Filter by date";
            this.chkDateFilter.UseVisualStyleBackColor = true;
            this.chkDateFilter.CheckedChanged += new System.EventHandler(this.chkDateFilter_CheckedChanged);
            //
            // lblDateFrom
            //
            this.lblDateFrom.AutoSize = true;
            this.lblDateFrom.Location = new System.Drawing.Point(20, 48);
            this.lblDateFrom.Name = "lblDateFrom";
            this.lblDateFrom.Size = new System.Drawing.Size(34, 13);
            this.lblDateFrom.TabIndex = 5;
            this.lblDateFrom.Text = "From:";
            //
            // dtpFrom
            //
            this.dtpFrom.Enabled = false;
            this.dtpFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFrom.Location = new System.Drawing.Point(90, 45);
            this.dtpFrom.Name = "dtpFrom";
            this.dtpFrom.Size = new System.Drawing.Size(110, 20);
            this.dtpFrom.TabIndex = 6;
            //
            // lblDateTo
            //
            this.lblDateTo.AutoSize = true;
            this.lblDateTo.Location = new System.Drawing.Point(210, 48);
            this.lblDateTo.Name = "lblDateTo";
            this.lblDateTo.Size = new System.Drawing.Size(22, 13);
            this.lblDateTo.TabIndex = 7;
            this.lblDateTo.Text = "To:";
            //
            // dtpTo
            //
            this.dtpTo.Enabled = false;
            this.dtpTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpTo.Location = new System.Drawing.Point(240, 45);
            this.dtpTo.Name = "dtpTo";
            this.dtpTo.Size = new System.Drawing.Size(110, 20);
            this.dtpTo.TabIndex = 8;
            //
            // lblSearch
            //
            this.lblSearch.AutoSize = true;
            this.lblSearch.Location = new System.Drawing.Point(360, 48);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(44, 13);
            this.lblSearch.TabIndex = 9;
            this.lblSearch.Text = "Search:";
            //
            // txtSearch
            //
            this.txtSearch.Location = new System.Drawing.Point(410, 45);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(200, 20);
            this.txtSearch.TabIndex = 10;
            //
            // btnSearch
            //
            this.btnSearch.Location = new System.Drawing.Point(620, 44);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(80, 24);
            this.btnSearch.TabIndex = 11;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            //
            // btnRefresh
            //
            this.btnRefresh.Location = new System.Drawing.Point(705, 44);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(80, 24);
            this.btnRefresh.TabIndex = 12;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            //
            // dgvStockOut
            //
            this.dgvStockOut.AllowUserToAddRows = false;
            this.dgvStockOut.AllowUserToDeleteRows = false;
            this.dgvStockOut.AllowUserToOrderColumns = false;
            this.dgvStockOut.AllowUserToResizeRows = false;
            this.dgvStockOut.AutoGenerateColumns = false;
            this.dgvStockOut.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvStockOut.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colStockOutID,
            this.colProductName,
            this.colCustomerName,
            this.colQuantity,
            this.colUnitPrice,
            this.colTotalPrice,
            this.colDateOut,
            this.colNotes});
            this.dgvStockOut.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvStockOut.Location = new System.Drawing.Point(0, 375);
            this.dgvStockOut.MultiSelect = false;
            this.dgvStockOut.Name = "dgvStockOut";
            this.dgvStockOut.ReadOnly = true;
            this.dgvStockOut.RowHeadersWidth = 25;
            this.dgvStockOut.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvStockOut.Size = new System.Drawing.Size(900, 300);
            this.dgvStockOut.TabIndex = 2;
            //
            // colStockOutID
            //
            this.colStockOutID.DataPropertyName = "StockOutID";
            this.colStockOutID.HeaderText = "ID";
            this.colStockOutID.Name = "colStockOutID";
            this.colStockOutID.ReadOnly = true;
            this.colStockOutID.Width = 50;
            //
            // colProductName
            //
            this.colProductName.DataPropertyName = "ProductName";
            this.colProductName.HeaderText = "Product";
            this.colProductName.Name = "colProductName";
            this.colProductName.ReadOnly = true;
            this.colProductName.Width = 170;
            //
            // colCustomerName
            //
            this.colCustomerName.DataPropertyName = "CustomerName";
            this.colCustomerName.HeaderText = "Customer";
            this.colCustomerName.Name = "colCustomerName";
            this.colCustomerName.ReadOnly = true;
            this.colCustomerName.Width = 140;
            //
            // colQuantity
            //
            this.colQuantity.DataPropertyName = "Quantity";
            this.colQuantity.HeaderText = "Quantity";
            this.colQuantity.Name = "colQuantity";
            this.colQuantity.ReadOnly = true;
            this.colQuantity.Width = 70;
            //
            // colUnitPrice
            //
            this.colUnitPrice.DataPropertyName = "UnitPrice";
            dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            dataGridViewCellStyle1.Format = "N2";
            this.colUnitPrice.DefaultCellStyle = dataGridViewCellStyle1;
            this.colUnitPrice.HeaderText = "Unit Price (USD)";
            this.colUnitPrice.Name = "colUnitPrice";
            this.colUnitPrice.ReadOnly = true;
            this.colUnitPrice.Width = 140;
            //
            // colTotalPrice
            //
            this.colTotalPrice.DataPropertyName = "TotalPrice";
            dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            dataGridViewCellStyle2.Format = "N2";
            this.colTotalPrice.DefaultCellStyle = dataGridViewCellStyle2;
            this.colTotalPrice.HeaderText = "Total Price (USD)";
            this.colTotalPrice.Name = "colTotalPrice";
            this.colTotalPrice.ReadOnly = true;
            this.colTotalPrice.Width = 145;
            //
            // colDateOut
            //
            this.colDateOut.DataPropertyName = "DateOut";
            dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            dataGridViewCellStyle3.Format = "yyyy-MM-dd HH:mm";
            this.colDateOut.DefaultCellStyle = dataGridViewCellStyle3;
            this.colDateOut.HeaderText = "Date Out";
            this.colDateOut.Name = "colDateOut";
            this.colDateOut.ReadOnly = true;
            this.colDateOut.Width = 130;
            //
            // colNotes
            //
            this.colNotes.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colNotes.DataPropertyName = "Notes";
            this.colNotes.HeaderText = "Notes";
            this.colNotes.Name = "colNotes";
            this.colNotes.ReadOnly = true;
            //
            // ucStockOut
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Size = new System.Drawing.Size(900, 675);
            this.Controls.Add(this.dgvStockOut);
            this.Controls.Add(this.pnlFilter);
            this.Controls.Add(this.pnlForm);
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(850, 600);
            this.Name = "ucStockOut";
            this.Load += new System.EventHandler(this.ucStockOut_Load);
            this.pnlForm.ResumeLayout(false);
            this.pnlForm.PerformLayout();
            this.pnlFilter.ResumeLayout(false);
            this.pnlFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStockOut)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1;
        private System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2;
        private System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3;

        private System.Windows.Forms.Panel pnlForm;
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.Label lblProduct;
        private System.Windows.Forms.ComboBox cmbProduct;
        private System.Windows.Forms.Label lblAvailableStock;
        private System.Windows.Forms.TextBox txtAvailableStock;
        private System.Windows.Forms.Label lblCustomer;
        private System.Windows.Forms.ComboBox cmbCustomer;
        private System.Windows.Forms.Label lblQuantity;
        private System.Windows.Forms.TextBox txtQuantity;
        private System.Windows.Forms.Label lblUnitPrice;
        private System.Windows.Forms.TextBox txtUnitPrice;
        private System.Windows.Forms.Label lblTotalPrice;
        private System.Windows.Forms.TextBox txtTotalPrice;
        private System.Windows.Forms.Label lblNotes;
        private System.Windows.Forms.TextBox txtNotes;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnClear;

        private System.Windows.Forms.Panel pnlFilter;
        private System.Windows.Forms.Label lblFilterProduct;
        private System.Windows.Forms.ComboBox cmbFilterProduct;
        private System.Windows.Forms.Label lblFilterCustomer;
        private System.Windows.Forms.ComboBox cmbFilterCustomer;
        private System.Windows.Forms.CheckBox chkDateFilter;
        private System.Windows.Forms.Label lblDateFrom;
        private System.Windows.Forms.DateTimePicker dtpFrom;
        private System.Windows.Forms.Label lblDateTo;
        private System.Windows.Forms.DateTimePicker dtpTo;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnRefresh;

        private System.Windows.Forms.DataGridView dgvStockOut;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStockOutID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProductName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCustomerName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colQuantity;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUnitPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTotalPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDateOut;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNotes;
    }
}
