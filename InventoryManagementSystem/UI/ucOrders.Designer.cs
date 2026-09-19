namespace InventoryManagementSystem.UI
{
    partial class ucOrders
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
            this.pnlOrdersTitle = new System.Windows.Forms.Panel();
            this.tabOrders = new System.Windows.Forms.TabControl();
            this.tpNewOrder = new System.Windows.Forms.TabPage();
            this.tpHistory = new System.Windows.Forms.TabPage();
            this.pnlHistoryActions = new System.Windows.Forms.Panel();
            this.pnlOrderInfo = new System.Windows.Forms.Panel();
            this.lblHeader = new System.Windows.Forms.Label();
            this.lblCustomer = new System.Windows.Forms.Label();
            this.cmbCustomer = new System.Windows.Forms.ComboBox();
            this.lblEmployee = new System.Windows.Forms.Label();
            this.cmbEmployee = new System.Windows.Forms.ComboBox();
            this.lblOrderDate = new System.Windows.Forms.Label();
            this.txtOrderDate = new System.Windows.Forms.TextBox();
            this.lblStatusCaption = new System.Windows.Forms.Label();
            this.lblStatusValue = new System.Windows.Forms.Label();
            this.pnlDetailEntry = new System.Windows.Forms.Panel();
            this.lblProduct = new System.Windows.Forms.Label();
            this.cmbProduct = new System.Windows.Forms.ComboBox();
            this.lblAvailableStock = new System.Windows.Forms.Label();
            this.txtAvailableStock = new System.Windows.Forms.TextBox();
            this.lblQuantity = new System.Windows.Forms.Label();
            this.txtQuantity = new System.Windows.Forms.TextBox();
            this.lblUnitPrice = new System.Windows.Forms.Label();
            this.txtUnitPrice = new System.Windows.Forms.TextBox();
            this.lblLineTotal = new System.Windows.Forms.Label();
            this.txtLineTotal = new System.Windows.Forms.TextBox();
            this.btnAddItem = new System.Windows.Forms.Button();
            this.btnRemoveItem = new System.Windows.Forms.Button();
            this.pnlDetailGrid = new System.Windows.Forms.Panel();
            this.dgvOrderDetails = new System.Windows.Forms.DataGridView();
            this.colDetailProduct = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDetailQuantity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDetailUnitPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDetailTotal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblTotalAmountCaption = new System.Windows.Forms.Label();
            this.txtTotalAmount = new System.Windows.Forms.TextBox();
            this.btnSaveOrder = new System.Windows.Forms.Button();
            this.btnConfirmOrder = new System.Windows.Forms.Button();
            this.btnCancelOrder = new System.Windows.Forms.Button();
            this.btnViewDetails = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.pnlFilter = new System.Windows.Forms.Panel();
            this.lblFilterStatus = new System.Windows.Forms.Label();
            this.cmbFilterStatus = new System.Windows.Forms.ComboBox();
            this.lblFilterCustomer = new System.Windows.Forms.Label();
            this.cmbFilterCustomer = new System.Windows.Forms.ComboBox();
            this.chkDateFilter = new System.Windows.Forms.CheckBox();
            this.lblDateFrom = new System.Windows.Forms.Label();
            this.dtpFrom = new System.Windows.Forms.DateTimePicker();
            this.lblDateTo = new System.Windows.Forms.Label();
            this.dtpTo = new System.Windows.Forms.DateTimePicker();
            this.lblSearch = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnSearchOrders = new System.Windows.Forms.Button();
            this.btnRefreshOrders = new System.Windows.Forms.Button();
            this.dgvHistory = new System.Windows.Forms.DataGridView();
            this.colHistOrderID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHistCustomer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHistEmployee = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHistOrderDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHistTotalAmount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHistStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlOrdersTitle.SuspendLayout();
            this.tabOrders.SuspendLayout();
            this.tpNewOrder.SuspendLayout();
            this.tpHistory.SuspendLayout();
            this.pnlHistoryActions.SuspendLayout();
            this.pnlOrderInfo.SuspendLayout();
            this.pnlDetailEntry.SuspendLayout();
            this.pnlDetailGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrderDetails)).BeginInit();
            this.pnlFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistory)).BeginInit();
            this.SuspendLayout();
            //
            // pnlHistoryActions
            //
            this.pnlHistoryActions.Controls.Add(this.btnConfirmOrder);
            this.pnlHistoryActions.Controls.Add(this.btnCancelOrder);
            this.pnlHistoryActions.Controls.Add(this.btnViewDetails);
            this.pnlHistoryActions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlHistoryActions.Name = "pnlHistoryActions";
            this.pnlHistoryActions.Size = new System.Drawing.Size(800, 54);
            this.pnlHistoryActions.TabIndex = 5;
            //
            // pnlOrdersTitle
            //
            this.pnlOrdersTitle.Controls.Add(this.lblHeader);
            this.pnlOrdersTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlOrdersTitle.Name = "pnlOrdersTitle";
            this.pnlOrdersTitle.Size = new System.Drawing.Size(800, 52);
            this.pnlOrdersTitle.TabIndex = 0;
            //
            // tabOrders
            //
            this.tabOrders.Controls.Add(this.tpNewOrder);
            this.tabOrders.Controls.Add(this.tpHistory);
            this.tabOrders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabOrders.ItemSize = new System.Drawing.Size(160, 34);
            this.tabOrders.Name = "tabOrders";
            this.tabOrders.SelectedIndex = 0;
            this.tabOrders.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabOrders.Size = new System.Drawing.Size(800, 508);
            this.tabOrders.TabIndex = 1;
            //
            // tpNewOrder
            //
            this.tpNewOrder.BackColor = System.Drawing.Color.White;
            this.tpNewOrder.Controls.Add(this.pnlDetailGrid);
            this.tpNewOrder.Controls.Add(this.pnlDetailEntry);
            this.tpNewOrder.Controls.Add(this.pnlOrderInfo);
            this.tpNewOrder.Name = "tpNewOrder";
            this.tpNewOrder.Size = new System.Drawing.Size(792, 466);
            this.tpNewOrder.TabIndex = 0;
            this.tpNewOrder.Text = "New Order";
            //
            // tpHistory
            //
            this.tpHistory.BackColor = System.Drawing.Color.White;
            this.tpHistory.Controls.Add(this.dgvHistory);
            this.tpHistory.Controls.Add(this.pnlHistoryActions);
            this.tpHistory.Controls.Add(this.pnlFilter);
            this.tpHistory.Name = "tpHistory";
            this.tpHistory.Size = new System.Drawing.Size(792, 466);
            this.tpHistory.TabIndex = 1;
            this.tpHistory.Text = "Order History";
            //
            // pnlOrderInfo
            //
            this.pnlOrderInfo.Controls.Add(this.lblCustomer);
            this.pnlOrderInfo.Controls.Add(this.cmbCustomer);
            this.pnlOrderInfo.Controls.Add(this.lblEmployee);
            this.pnlOrderInfo.Controls.Add(this.cmbEmployee);
            this.pnlOrderInfo.Controls.Add(this.lblOrderDate);
            this.pnlOrderInfo.Controls.Add(this.txtOrderDate);
            this.pnlOrderInfo.Controls.Add(this.lblStatusCaption);
            this.pnlOrderInfo.Controls.Add(this.lblStatusValue);
            this.pnlOrderInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlOrderInfo.Location = new System.Drawing.Point(0, 0);
            this.pnlOrderInfo.Name = "pnlOrderInfo";
            this.pnlOrderInfo.Size = new System.Drawing.Size(800, 84);
            this.pnlOrderInfo.TabIndex = 0;
            //
            // lblHeader
            //
            this.lblHeader.AutoSize = true;
            this.lblHeader.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.lblHeader.Location = new System.Drawing.Point(20, 8);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(75, 25);
            this.lblHeader.TabIndex = 0;
            this.lblHeader.Text = "Orders";
            //
            // lblCustomer
            //
            this.lblCustomer.AutoSize = true;
            this.lblCustomer.Location = new System.Drawing.Point(20, 18);
            this.lblCustomer.Name = "lblCustomer";
            this.lblCustomer.Size = new System.Drawing.Size(55, 13);
            this.lblCustomer.TabIndex = 1;
            this.lblCustomer.Text = "Customer:";
            //
            // cmbCustomer
            //
            this.cmbCustomer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCustomer.FormattingEnabled = true;
            this.cmbCustomer.Location = new System.Drawing.Point(100, 15);
            this.cmbCustomer.Name = "cmbCustomer";
            this.cmbCustomer.Size = new System.Drawing.Size(220, 21);
            this.cmbCustomer.TabIndex = 2;
            //
            // lblEmployee
            //
            this.lblEmployee.AutoSize = true;
            this.lblEmployee.Location = new System.Drawing.Point(350, 18);
            this.lblEmployee.Name = "lblEmployee";
            this.lblEmployee.Size = new System.Drawing.Size(56, 13);
            this.lblEmployee.TabIndex = 3;
            this.lblEmployee.Text = "Employee:";
            //
            // cmbEmployee
            //
            this.cmbEmployee.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEmployee.FormattingEnabled = true;
            this.cmbEmployee.Location = new System.Drawing.Point(430, 15);
            this.cmbEmployee.Name = "cmbEmployee";
            this.cmbEmployee.Size = new System.Drawing.Size(220, 21);
            this.cmbEmployee.TabIndex = 4;
            //
            // lblOrderDate
            //
            this.lblOrderDate.AutoSize = true;
            this.lblOrderDate.Location = new System.Drawing.Point(20, 52);
            this.lblOrderDate.Name = "lblOrderDate";
            this.lblOrderDate.Size = new System.Drawing.Size(63, 13);
            this.lblOrderDate.TabIndex = 5;
            this.lblOrderDate.Text = "Order Date:";
            //
            // txtOrderDate
            //
            this.txtOrderDate.Location = new System.Drawing.Point(100, 49);
            this.txtOrderDate.Name = "txtOrderDate";
            this.txtOrderDate.ReadOnly = true;
            this.txtOrderDate.Size = new System.Drawing.Size(150, 20);
            this.txtOrderDate.TabIndex = 6;
            this.txtOrderDate.TabStop = false;
            //
            // lblStatusCaption
            //
            this.lblStatusCaption.AutoSize = true;
            this.lblStatusCaption.Location = new System.Drawing.Point(350, 52);
            this.lblStatusCaption.Name = "lblStatusCaption";
            this.lblStatusCaption.Size = new System.Drawing.Size(41, 13);
            this.lblStatusCaption.TabIndex = 7;
            this.lblStatusCaption.Text = "Status:";
            //
            // lblStatusValue
            //
            this.lblStatusValue.AutoSize = true;
            this.lblStatusValue.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblStatusValue.Location = new System.Drawing.Point(430, 52);
            this.lblStatusValue.Name = "lblStatusValue";
            this.lblStatusValue.Size = new System.Drawing.Size(46, 13);
            this.lblStatusValue.TabIndex = 8;
            this.lblStatusValue.Text = "Pending";
            //
            // pnlDetailEntry
            //
            this.pnlDetailEntry.Controls.Add(this.lblProduct);
            this.pnlDetailEntry.Controls.Add(this.cmbProduct);
            this.pnlDetailEntry.Controls.Add(this.lblAvailableStock);
            this.pnlDetailEntry.Controls.Add(this.txtAvailableStock);
            this.pnlDetailEntry.Controls.Add(this.lblQuantity);
            this.pnlDetailEntry.Controls.Add(this.txtQuantity);
            this.pnlDetailEntry.Controls.Add(this.lblUnitPrice);
            this.pnlDetailEntry.Controls.Add(this.txtUnitPrice);
            this.pnlDetailEntry.Controls.Add(this.lblLineTotal);
            this.pnlDetailEntry.Controls.Add(this.txtLineTotal);
            this.pnlDetailEntry.Controls.Add(this.btnAddItem);
            this.pnlDetailEntry.Controls.Add(this.btnRemoveItem);
            this.pnlDetailEntry.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlDetailEntry.Location = new System.Drawing.Point(0, 110);
            this.pnlDetailEntry.Name = "pnlDetailEntry";
            this.pnlDetailEntry.Size = new System.Drawing.Size(800, 96);
            this.pnlDetailEntry.TabIndex = 1;
            //
            // lblProduct
            //
            this.lblProduct.AutoSize = true;
            this.lblProduct.Location = new System.Drawing.Point(20, 15);
            this.lblProduct.Name = "lblProduct";
            this.lblProduct.Size = new System.Drawing.Size(45, 13);
            this.lblProduct.TabIndex = 0;
            this.lblProduct.Text = "Product:";
            //
            // cmbProduct
            //
            this.cmbProduct.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProduct.FormattingEnabled = true;
            this.cmbProduct.Location = new System.Drawing.Point(100, 12);
            this.cmbProduct.Name = "cmbProduct";
            this.cmbProduct.Size = new System.Drawing.Size(250, 21);
            this.cmbProduct.TabIndex = 1;
            this.cmbProduct.SelectedIndexChanged += new System.EventHandler(this.cmbProduct_SelectedIndexChanged);
            //
            // lblAvailableStock
            //
            this.lblAvailableStock.AutoSize = true;
            this.lblAvailableStock.Location = new System.Drawing.Point(370, 15);
            this.lblAvailableStock.Name = "lblAvailableStock";
            this.lblAvailableStock.Size = new System.Drawing.Size(88, 13);
            this.lblAvailableStock.TabIndex = 2;
            this.lblAvailableStock.Text = "Available Stock:";
            //
            // txtAvailableStock
            //
            this.txtAvailableStock.Location = new System.Drawing.Point(470, 12);
            this.txtAvailableStock.Name = "txtAvailableStock";
            this.txtAvailableStock.ReadOnly = true;
            this.txtAvailableStock.Size = new System.Drawing.Size(80, 20);
            this.txtAvailableStock.TabIndex = 3;
            this.txtAvailableStock.TabStop = false;
            //
            // lblQuantity
            //
            this.lblQuantity.AutoSize = true;
            this.lblQuantity.Location = new System.Drawing.Point(570, 15);
            this.lblQuantity.Name = "lblQuantity";
            this.lblQuantity.Size = new System.Drawing.Size(52, 13);
            this.lblQuantity.TabIndex = 4;
            this.lblQuantity.Text = "Quantity:";
            //
            // txtQuantity
            //
            this.txtQuantity.Location = new System.Drawing.Point(640, 12);
            this.txtQuantity.Name = "txtQuantity";
            this.txtQuantity.Size = new System.Drawing.Size(70, 20);
            this.txtQuantity.TabIndex = 5;
            this.txtQuantity.TextChanged += new System.EventHandler(this.QuantityOrUnitPrice_TextChanged);
            //
            // lblUnitPrice
            //
            this.lblUnitPrice.AutoSize = true;
            this.lblUnitPrice.Location = new System.Drawing.Point(20, 55);
            this.lblUnitPrice.Name = "lblUnitPrice";
            this.lblUnitPrice.Size = new System.Drawing.Size(58, 13);
            this.lblUnitPrice.TabIndex = 6;
            this.lblUnitPrice.Text = "Unit Price (USD):";
            //
            // txtUnitPrice
            //
            this.txtUnitPrice.Location = new System.Drawing.Point(140, 52);
            this.txtUnitPrice.Name = "txtUnitPrice";
            this.txtUnitPrice.Size = new System.Drawing.Size(90, 20);
            this.txtUnitPrice.TabIndex = 7;
            this.txtUnitPrice.TextChanged += new System.EventHandler(this.QuantityOrUnitPrice_TextChanged);
            //
            // lblLineTotal
            //
            this.lblLineTotal.AutoSize = true;
            this.lblLineTotal.Location = new System.Drawing.Point(245, 55);
            this.lblLineTotal.Name = "lblLineTotal";
            this.lblLineTotal.Size = new System.Drawing.Size(58, 13);
            this.lblLineTotal.TabIndex = 8;
            this.lblLineTotal.Text = "Line Total (USD):";
            //
            // txtLineTotal
            //
            this.txtLineTotal.Location = new System.Drawing.Point(360, 52);
            this.txtLineTotal.Name = "txtLineTotal";
            this.txtLineTotal.ReadOnly = true;
            this.txtLineTotal.Size = new System.Drawing.Size(100, 20);
            this.txtLineTotal.TabIndex = 9;
            this.txtLineTotal.TabStop = false;
            //
            // btnAddItem
            //
            this.btnAddItem.Location = new System.Drawing.Point(490, 48);
            this.btnAddItem.Name = "btnAddItem";
            this.btnAddItem.Size = new System.Drawing.Size(110, 32);
            this.btnAddItem.TabIndex = 10;
            this.btnAddItem.Text = "Add Item";
            this.btnAddItem.UseVisualStyleBackColor = true;
            this.btnAddItem.Click += new System.EventHandler(this.btnAddItem_Click);
            //
            // btnRemoveItem
            //
            this.btnRemoveItem.Location = new System.Drawing.Point(610, 48);
            this.btnRemoveItem.Name = "btnRemoveItem";
            this.btnRemoveItem.Size = new System.Drawing.Size(120, 32);
            this.btnRemoveItem.TabIndex = 11;
            this.btnRemoveItem.Text = "Remove Item";
            this.btnRemoveItem.UseVisualStyleBackColor = true;
            this.btnRemoveItem.Click += new System.EventHandler(this.btnRemoveItem_Click);
            //
            // pnlDetailGrid
            //
            this.pnlDetailGrid.Controls.Add(this.dgvOrderDetails);
            this.pnlDetailGrid.Controls.Add(this.lblTotalAmountCaption);
            this.pnlDetailGrid.Controls.Add(this.txtTotalAmount);
            this.pnlDetailGrid.Controls.Add(this.btnSaveOrder);
            this.pnlDetailGrid.Controls.Add(this.btnClear);
            this.pnlDetailGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDetailGrid.Location = new System.Drawing.Point(0, 205);
            this.pnlDetailGrid.Name = "pnlDetailGrid";
            this.pnlDetailGrid.Size = new System.Drawing.Size(800, 300);
            this.pnlDetailGrid.TabIndex = 2;
            //
            // dgvOrderDetails
            //
            this.dgvOrderDetails.AllowUserToAddRows = false;
            this.dgvOrderDetails.AllowUserToDeleteRows = false;
            this.dgvOrderDetails.AllowUserToOrderColumns = false;
            this.dgvOrderDetails.AllowUserToResizeRows = false;
            this.dgvOrderDetails.AutoGenerateColumns = false;
            this.dgvOrderDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvOrderDetails.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colDetailProduct,
            this.colDetailQuantity,
            this.colDetailUnitPrice,
            this.colDetailTotal});
            this.dgvOrderDetails.Location = new System.Drawing.Point(10, 10);
            this.dgvOrderDetails.MultiSelect = false;
            this.dgvOrderDetails.Name = "dgvOrderDetails";
            this.dgvOrderDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvOrderDetails.ReadOnly = true;
            this.dgvOrderDetails.RowHeadersWidth = 25;
            this.dgvOrderDetails.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvOrderDetails.Size = new System.Drawing.Size(780, 200);
            this.dgvOrderDetails.TabIndex = 0;
            //
            // colDetailProduct
            //
            this.colDetailProduct.DataPropertyName = "ProductName";
            this.colDetailProduct.HeaderText = "Product";
            this.colDetailProduct.Name = "colDetailProduct";
            this.colDetailProduct.ReadOnly = true;
            this.colDetailProduct.Width = 300;
            //
            // colDetailQuantity
            //
            this.colDetailQuantity.DataPropertyName = "Quantity";
            this.colDetailQuantity.HeaderText = "Quantity";
            this.colDetailQuantity.Name = "colDetailQuantity";
            this.colDetailQuantity.ReadOnly = true;
            this.colDetailQuantity.Width = 150;
            //
            // colDetailUnitPrice
            //
            this.colDetailUnitPrice.DataPropertyName = "UnitPrice";
            dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            dataGridViewCellStyle1.Format = "N2";
            this.colDetailUnitPrice.DefaultCellStyle = dataGridViewCellStyle1;
            this.colDetailUnitPrice.HeaderText = "Unit Price (USD)";
            this.colDetailUnitPrice.Name = "colDetailUnitPrice";
            this.colDetailUnitPrice.ReadOnly = true;
            this.colDetailUnitPrice.Width = 150;
            //
            // colDetailTotal
            //
            this.colDetailTotal.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colDetailTotal.DataPropertyName = "Total";
            dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            dataGridViewCellStyle2.Format = "N2";
            this.colDetailTotal.DefaultCellStyle = dataGridViewCellStyle2;
            this.colDetailTotal.HeaderText = "Line Total (USD)";
            this.colDetailTotal.Name = "colDetailTotal";
            this.colDetailTotal.ReadOnly = true;
            //
            // lblTotalAmountCaption
            //
            this.lblTotalAmountCaption.AutoSize = true;
            this.lblTotalAmountCaption.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblTotalAmountCaption.Location = new System.Drawing.Point(495, 236);
            this.lblTotalAmountCaption.Name = "lblTotalAmountCaption";
            this.lblTotalAmountCaption.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTotalAmountCaption.Size = new System.Drawing.Size(88, 13);
            this.lblTotalAmountCaption.TabIndex = 1;
            this.lblTotalAmountCaption.Text = "Total Amount (USD):";
            //
            // txtTotalAmount
            //
            this.txtTotalAmount.Location = new System.Drawing.Point(640, 233);
            this.txtTotalAmount.Name = "txtTotalAmount";
            this.txtTotalAmount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTotalAmount.ReadOnly = true;
            this.txtTotalAmount.Size = new System.Drawing.Size(120, 20);
            this.txtTotalAmount.TabIndex = 2;
            this.txtTotalAmount.TabStop = false;
            //
            // btnSaveOrder
            //
            this.btnSaveOrder.Location = new System.Drawing.Point(10, 230);
            this.btnSaveOrder.Name = "btnSaveOrder";
            this.btnSaveOrder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSaveOrder.Size = new System.Drawing.Size(160, 32);
            this.btnSaveOrder.TabIndex = 3;
            this.btnSaveOrder.Text = "Save Pending Order";
            this.btnSaveOrder.UseVisualStyleBackColor = true;
            this.btnSaveOrder.Click += new System.EventHandler(this.btnSaveOrder_Click);
            //
            // btnConfirmOrder
            //
            this.btnConfirmOrder.Location = new System.Drawing.Point(10, 10);
            this.btnConfirmOrder.Name = "btnConfirmOrder";
            this.btnConfirmOrder.Size = new System.Drawing.Size(120, 32);
            this.btnConfirmOrder.TabIndex = 4;
            this.btnConfirmOrder.Text = "Confirm Order";
            this.btnConfirmOrder.UseVisualStyleBackColor = true;
            this.btnConfirmOrder.Enabled = false;
            this.btnConfirmOrder.Click += new System.EventHandler(this.btnConfirmOrder_Click);
            //
            // btnCancelOrder
            //
            this.btnCancelOrder.Location = new System.Drawing.Point(140, 10);
            this.btnCancelOrder.Name = "btnCancelOrder";
            this.btnCancelOrder.Size = new System.Drawing.Size(120, 32);
            this.btnCancelOrder.TabIndex = 5;
            this.btnCancelOrder.Text = "Cancel Order";
            this.btnCancelOrder.UseVisualStyleBackColor = true;
            this.btnCancelOrder.Enabled = false;
            this.btnCancelOrder.Click += new System.EventHandler(this.btnCancelOrder_Click);
            //
            // btnViewDetails
            //
            this.btnViewDetails.Location = new System.Drawing.Point(270, 10);
            this.btnViewDetails.Name = "btnViewDetails";
            this.btnViewDetails.Size = new System.Drawing.Size(120, 32);
            this.btnViewDetails.TabIndex = 6;
            this.btnViewDetails.Text = "View Details";
            this.btnViewDetails.UseVisualStyleBackColor = true;
            this.btnViewDetails.Enabled = false;
            this.btnViewDetails.Click += new System.EventHandler(this.btnViewDetails_Click);
            //
            // btnClear
            //
            this.btnClear.Location = new System.Drawing.Point(180, 230);
            this.btnClear.Name = "btnClear";
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClear.Size = new System.Drawing.Size(90, 32);
            this.btnClear.TabIndex = 7;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            //
            // pnlFilter
            //
            this.pnlFilter.Controls.Add(this.lblFilterStatus);
            this.pnlFilter.Controls.Add(this.cmbFilterStatus);
            this.pnlFilter.Controls.Add(this.lblFilterCustomer);
            this.pnlFilter.Controls.Add(this.cmbFilterCustomer);
            this.pnlFilter.Controls.Add(this.chkDateFilter);
            this.pnlFilter.Controls.Add(this.lblDateFrom);
            this.pnlFilter.Controls.Add(this.dtpFrom);
            this.pnlFilter.Controls.Add(this.lblDateTo);
            this.pnlFilter.Controls.Add(this.dtpTo);
            this.pnlFilter.Controls.Add(this.lblSearch);
            this.pnlFilter.Controls.Add(this.txtSearch);
            this.pnlFilter.Controls.Add(this.btnSearchOrders);
            this.pnlFilter.Controls.Add(this.btnRefreshOrders);
            this.pnlFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFilter.Location = new System.Drawing.Point(0, 430);
            this.pnlFilter.Name = "pnlFilter";
            this.pnlFilter.Size = new System.Drawing.Size(800, 90);
            this.pnlFilter.TabIndex = 3;
            //
            // lblFilterStatus
            //
            this.lblFilterStatus.AutoSize = true;
            this.lblFilterStatus.Location = new System.Drawing.Point(20, 13);
            this.lblFilterStatus.Name = "lblFilterStatus";
            this.lblFilterStatus.Size = new System.Drawing.Size(41, 13);
            this.lblFilterStatus.TabIndex = 0;
            this.lblFilterStatus.Text = "Status:";
            //
            // cmbFilterStatus
            //
            this.cmbFilterStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFilterStatus.FormattingEnabled = true;
            this.cmbFilterStatus.Location = new System.Drawing.Point(70, 10);
            this.cmbFilterStatus.Name = "cmbFilterStatus";
            this.cmbFilterStatus.Size = new System.Drawing.Size(150, 21);
            this.cmbFilterStatus.TabIndex = 1;
            //
            // lblFilterCustomer
            //
            this.lblFilterCustomer.AutoSize = true;
            this.lblFilterCustomer.Location = new System.Drawing.Point(240, 13);
            this.lblFilterCustomer.Name = "lblFilterCustomer";
            this.lblFilterCustomer.Size = new System.Drawing.Size(58, 13);
            this.lblFilterCustomer.TabIndex = 2;
            this.lblFilterCustomer.Text = "Customer:";
            //
            // cmbFilterCustomer
            //
            this.cmbFilterCustomer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFilterCustomer.FormattingEnabled = true;
            this.cmbFilterCustomer.Location = new System.Drawing.Point(310, 10);
            this.cmbFilterCustomer.Name = "cmbFilterCustomer";
            this.cmbFilterCustomer.Size = new System.Drawing.Size(180, 21);
            this.cmbFilterCustomer.TabIndex = 3;
            //
            // chkDateFilter
            //
            this.chkDateFilter.AutoSize = true;
            this.chkDateFilter.Location = new System.Drawing.Point(510, 12);
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
            this.dtpFrom.Location = new System.Drawing.Point(70, 45);
            this.dtpFrom.Name = "dtpFrom";
            this.dtpFrom.Size = new System.Drawing.Size(110, 20);
            this.dtpFrom.TabIndex = 6;
            //
            // lblDateTo
            //
            this.lblDateTo.AutoSize = true;
            this.lblDateTo.Location = new System.Drawing.Point(190, 48);
            this.lblDateTo.Name = "lblDateTo";
            this.lblDateTo.Size = new System.Drawing.Size(22, 13);
            this.lblDateTo.TabIndex = 7;
            this.lblDateTo.Text = "To:";
            //
            // dtpTo
            //
            this.dtpTo.Enabled = false;
            this.dtpTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpTo.Location = new System.Drawing.Point(220, 45);
            this.dtpTo.Name = "dtpTo";
            this.dtpTo.Size = new System.Drawing.Size(110, 20);
            this.dtpTo.TabIndex = 8;
            //
            // lblSearch
            //
            this.lblSearch.AutoSize = true;
            this.lblSearch.Location = new System.Drawing.Point(350, 48);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(44, 13);
            this.lblSearch.TabIndex = 9;
            this.lblSearch.Text = "Search:";
            //
            // txtSearch
            //
            this.txtSearch.Location = new System.Drawing.Point(400, 45);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(200, 20);
            this.txtSearch.TabIndex = 10;
            //
            // btnSearchOrders
            //
            this.btnSearchOrders.Location = new System.Drawing.Point(610, 44);
            this.btnSearchOrders.Name = "btnSearchOrders";
            this.btnSearchOrders.Size = new System.Drawing.Size(80, 24);
            this.btnSearchOrders.TabIndex = 11;
            this.btnSearchOrders.Text = "Search";
            this.btnSearchOrders.UseVisualStyleBackColor = true;
            this.btnSearchOrders.Click += new System.EventHandler(this.btnSearchOrders_Click);
            //
            // btnRefreshOrders
            //
            this.btnRefreshOrders.Location = new System.Drawing.Point(695, 44);
            this.btnRefreshOrders.Name = "btnRefreshOrders";
            this.btnRefreshOrders.Size = new System.Drawing.Size(80, 24);
            this.btnRefreshOrders.TabIndex = 12;
            this.btnRefreshOrders.Text = "Refresh";
            this.btnRefreshOrders.UseVisualStyleBackColor = true;
            this.btnRefreshOrders.Click += new System.EventHandler(this.btnRefreshOrders_Click);
            //
            // dgvHistory
            //
            this.dgvHistory.AllowUserToAddRows = false;
            this.dgvHistory.AllowUserToDeleteRows = false;
            this.dgvHistory.AllowUserToOrderColumns = false;
            this.dgvHistory.AllowUserToResizeRows = false;
            this.dgvHistory.AutoGenerateColumns = false;
            this.dgvHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvHistory.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colHistOrderID,
            this.colHistCustomer,
            this.colHistEmployee,
            this.colHistOrderDate,
            this.colHistTotalAmount,
            this.colHistStatus});
            this.dgvHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvHistory.Location = new System.Drawing.Point(0, 520);
            this.dgvHistory.MultiSelect = false;
            this.dgvHistory.Name = "dgvHistory";
            this.dgvHistory.ReadOnly = true;
            this.dgvHistory.RowHeadersWidth = 25;
            this.dgvHistory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvHistory.Size = new System.Drawing.Size(1000, 280);
            this.dgvHistory.TabIndex = 4;
            this.dgvHistory.SelectionChanged += new System.EventHandler(this.dgvHistory_SelectionChanged);
            this.dgvHistory.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvHistory_CellDoubleClick);
            //
            // colHistOrderID
            //
            this.colHistOrderID.DataPropertyName = "OrderID";
            this.colHistOrderID.HeaderText = "ID";
            this.colHistOrderID.Name = "colHistOrderID";
            this.colHistOrderID.ReadOnly = true;
            this.colHistOrderID.Width = 60;
            //
            // colHistCustomer
            //
            this.colHistCustomer.DataPropertyName = "CustomerName";
            this.colHistCustomer.HeaderText = "Customer";
            this.colHistCustomer.Name = "colHistCustomer";
            this.colHistCustomer.ReadOnly = true;
            this.colHistCustomer.Width = 160;
            //
            // colHistEmployee
            //
            this.colHistEmployee.DataPropertyName = "EmployeeName";
            this.colHistEmployee.HeaderText = "Employee";
            this.colHistEmployee.Name = "colHistEmployee";
            this.colHistEmployee.ReadOnly = true;
            this.colHistEmployee.Width = 160;
            //
            // colHistOrderDate
            //
            this.colHistOrderDate.DataPropertyName = "OrderDate";
            dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            dataGridViewCellStyle3.Format = "yyyy-MM-dd HH:mm";
            this.colHistOrderDate.DefaultCellStyle = dataGridViewCellStyle3;
            this.colHistOrderDate.HeaderText = "Order Date";
            this.colHistOrderDate.Name = "colHistOrderDate";
            this.colHistOrderDate.ReadOnly = true;
            this.colHistOrderDate.Width = 140;
            //
            // colHistTotalAmount
            //
            this.colHistTotalAmount.DataPropertyName = "TotalAmount";
            dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            dataGridViewCellStyle4.Format = "N2";
            this.colHistTotalAmount.DefaultCellStyle = dataGridViewCellStyle4;
            this.colHistTotalAmount.HeaderText = "Total Amount (USD)";
            this.colHistTotalAmount.Name = "colHistTotalAmount";
            this.colHistTotalAmount.ReadOnly = true;
            this.colHistTotalAmount.Width = 165;
            //
            // colHistStatus
            //
            this.colHistStatus.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colHistStatus.DataPropertyName = "Status";
            this.colHistStatus.HeaderText = "Status";
            this.colHistStatus.Name = "colHistStatus";
            this.colHistStatus.ReadOnly = true;
            //
            // ucOrders
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Size = new System.Drawing.Size(800, 560);
            this.Controls.Add(this.tabOrders);
            this.Controls.Add(this.pnlOrdersTitle);
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(720, 520);
            this.Name = "ucOrders";
            this.Load += new System.EventHandler(this.ucOrders_Load);
            this.pnlOrdersTitle.ResumeLayout(false);
            this.tabOrders.ResumeLayout(false);
            this.tpNewOrder.ResumeLayout(false);
            this.tpHistory.ResumeLayout(false);
            this.pnlHistoryActions.ResumeLayout(false);
            this.pnlOrderInfo.ResumeLayout(false);
            this.pnlOrderInfo.PerformLayout();
            this.pnlDetailEntry.ResumeLayout(false);
            this.pnlDetailEntry.PerformLayout();
            this.pnlDetailGrid.ResumeLayout(false);
            this.pnlDetailGrid.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrderDetails)).EndInit();
            this.pnlFilter.ResumeLayout(false);
            this.pnlFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistory)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1;
        private System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2;
        private System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3;
        private System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4;

        private System.Windows.Forms.Panel pnlOrdersTitle;
        private System.Windows.Forms.TabControl tabOrders;
        private System.Windows.Forms.TabPage tpNewOrder;
        private System.Windows.Forms.TabPage tpHistory;
        private System.Windows.Forms.Panel pnlHistoryActions;
        private System.Windows.Forms.Panel pnlOrderInfo;
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.Label lblCustomer;
        private System.Windows.Forms.ComboBox cmbCustomer;
        private System.Windows.Forms.Label lblEmployee;
        private System.Windows.Forms.ComboBox cmbEmployee;
        private System.Windows.Forms.Label lblOrderDate;
        private System.Windows.Forms.TextBox txtOrderDate;
        private System.Windows.Forms.Label lblStatusCaption;
        private System.Windows.Forms.Label lblStatusValue;

        private System.Windows.Forms.Panel pnlDetailEntry;
        private System.Windows.Forms.Label lblProduct;
        private System.Windows.Forms.ComboBox cmbProduct;
        private System.Windows.Forms.Label lblAvailableStock;
        private System.Windows.Forms.TextBox txtAvailableStock;
        private System.Windows.Forms.Label lblQuantity;
        private System.Windows.Forms.TextBox txtQuantity;
        private System.Windows.Forms.Label lblUnitPrice;
        private System.Windows.Forms.TextBox txtUnitPrice;
        private System.Windows.Forms.Label lblLineTotal;
        private System.Windows.Forms.TextBox txtLineTotal;
        private System.Windows.Forms.Button btnAddItem;
        private System.Windows.Forms.Button btnRemoveItem;

        private System.Windows.Forms.Panel pnlDetailGrid;
        private System.Windows.Forms.DataGridView dgvOrderDetails;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDetailProduct;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDetailQuantity;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDetailUnitPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDetailTotal;
        private System.Windows.Forms.Label lblTotalAmountCaption;
        private System.Windows.Forms.TextBox txtTotalAmount;
        private System.Windows.Forms.Button btnSaveOrder;
        private System.Windows.Forms.Button btnConfirmOrder;
        private System.Windows.Forms.Button btnCancelOrder;
        private System.Windows.Forms.Button btnViewDetails;
        private System.Windows.Forms.Button btnClear;

        private System.Windows.Forms.Panel pnlFilter;
        private System.Windows.Forms.Label lblFilterStatus;
        private System.Windows.Forms.ComboBox cmbFilterStatus;
        private System.Windows.Forms.Label lblFilterCustomer;
        private System.Windows.Forms.ComboBox cmbFilterCustomer;
        private System.Windows.Forms.CheckBox chkDateFilter;
        private System.Windows.Forms.Label lblDateFrom;
        private System.Windows.Forms.DateTimePicker dtpFrom;
        private System.Windows.Forms.Label lblDateTo;
        private System.Windows.Forms.DateTimePicker dtpTo;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnSearchOrders;
        private System.Windows.Forms.Button btnRefreshOrders;

        private System.Windows.Forms.DataGridView dgvHistory;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHistOrderID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHistCustomer;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHistEmployee;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHistOrderDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHistTotalAmount;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHistStatus;
    }
}
