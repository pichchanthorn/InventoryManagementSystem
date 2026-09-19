namespace InventoryManagementSystem.UI
{
    partial class frmOrderDetails
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
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblOrderIdCaption = new System.Windows.Forms.Label();
            this.txtOrderId = new System.Windows.Forms.TextBox();
            this.lblStatusCaption = new System.Windows.Forms.Label();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.lblOrderDateCaption = new System.Windows.Forms.Label();
            this.txtOrderDate = new System.Windows.Forms.TextBox();
            this.lblCustomerCaption = new System.Windows.Forms.Label();
            this.txtCustomerName = new System.Windows.Forms.TextBox();
            this.lblEmployeeCaption = new System.Windows.Forms.Label();
            this.txtEmployeeName = new System.Windows.Forms.TextBox();
            this.lblTotalAmountCaption = new System.Windows.Forms.Label();
            this.txtTotalAmount = new System.Windows.Forms.TextBox();
            this.lblItemsHeader = new System.Windows.Forms.Label();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.dgvItems = new System.Windows.Forms.DataGridView();
            this.colItemProduct = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colItemQuantity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colItemUnitPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colItemTotal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlHeader.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).BeginInit();
            this.SuspendLayout();
            //
            // pnlHeader
            //
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Controls.Add(this.lblOrderIdCaption);
            this.pnlHeader.Controls.Add(this.txtOrderId);
            this.pnlHeader.Controls.Add(this.lblStatusCaption);
            this.pnlHeader.Controls.Add(this.txtStatus);
            this.pnlHeader.Controls.Add(this.lblOrderDateCaption);
            this.pnlHeader.Controls.Add(this.txtOrderDate);
            this.pnlHeader.Controls.Add(this.lblCustomerCaption);
            this.pnlHeader.Controls.Add(this.txtCustomerName);
            this.pnlHeader.Controls.Add(this.lblEmployeeCaption);
            this.pnlHeader.Controls.Add(this.txtEmployeeName);
            this.pnlHeader.Controls.Add(this.lblTotalAmountCaption);
            this.pnlHeader.Controls.Add(this.txtTotalAmount);
            this.pnlHeader.Controls.Add(this.lblItemsHeader);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(700, 190);
            this.pnlHeader.TabIndex = 0;
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(20, 10);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(120, 25);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Order Details";
            //
            // lblOrderIdCaption
            //
            this.lblOrderIdCaption.AutoSize = true;
            this.lblOrderIdCaption.Location = new System.Drawing.Point(20, 53);
            this.lblOrderIdCaption.Name = "lblOrderIdCaption";
            this.lblOrderIdCaption.Size = new System.Drawing.Size(53, 13);
            this.lblOrderIdCaption.TabIndex = 1;
            this.lblOrderIdCaption.Text = "Order ID:";
            //
            // txtOrderId
            //
            this.txtOrderId.Location = new System.Drawing.Point(100, 50);
            this.txtOrderId.Name = "txtOrderId";
            this.txtOrderId.ReadOnly = true;
            this.txtOrderId.Size = new System.Drawing.Size(70, 20);
            this.txtOrderId.TabIndex = 2;
            this.txtOrderId.TabStop = false;
            //
            // lblStatusCaption
            //
            this.lblStatusCaption.AutoSize = true;
            this.lblStatusCaption.Location = new System.Drawing.Point(190, 53);
            this.lblStatusCaption.Name = "lblStatusCaption";
            this.lblStatusCaption.Size = new System.Drawing.Size(41, 13);
            this.lblStatusCaption.TabIndex = 3;
            this.lblStatusCaption.Text = "Status:";
            //
            // txtStatus
            //
            this.txtStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.txtStatus.Location = new System.Drawing.Point(250, 50);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ReadOnly = true;
            this.txtStatus.Size = new System.Drawing.Size(110, 20);
            this.txtStatus.TabIndex = 4;
            this.txtStatus.TabStop = false;
            //
            // lblOrderDateCaption
            //
            this.lblOrderDateCaption.AutoSize = true;
            this.lblOrderDateCaption.Location = new System.Drawing.Point(380, 53);
            this.lblOrderDateCaption.Name = "lblOrderDateCaption";
            this.lblOrderDateCaption.Size = new System.Drawing.Size(63, 13);
            this.lblOrderDateCaption.TabIndex = 5;
            this.lblOrderDateCaption.Text = "Order Date:";
            //
            // txtOrderDate
            //
            this.txtOrderDate.Location = new System.Drawing.Point(470, 50);
            this.txtOrderDate.Name = "txtOrderDate";
            this.txtOrderDate.ReadOnly = true;
            this.txtOrderDate.Size = new System.Drawing.Size(150, 20);
            this.txtOrderDate.TabIndex = 6;
            this.txtOrderDate.TabStop = false;
            //
            // lblCustomerCaption
            //
            this.lblCustomerCaption.AutoSize = true;
            this.lblCustomerCaption.Location = new System.Drawing.Point(20, 88);
            this.lblCustomerCaption.Name = "lblCustomerCaption";
            this.lblCustomerCaption.Size = new System.Drawing.Size(55, 13);
            this.lblCustomerCaption.TabIndex = 7;
            this.lblCustomerCaption.Text = "Customer:";
            //
            // txtCustomerName
            //
            this.txtCustomerName.Location = new System.Drawing.Point(100, 85);
            this.txtCustomerName.Name = "txtCustomerName";
            this.txtCustomerName.ReadOnly = true;
            this.txtCustomerName.Size = new System.Drawing.Size(250, 20);
            this.txtCustomerName.TabIndex = 8;
            this.txtCustomerName.TabStop = false;
            //
            // lblEmployeeCaption
            //
            this.lblEmployeeCaption.AutoSize = true;
            this.lblEmployeeCaption.Location = new System.Drawing.Point(370, 88);
            this.lblEmployeeCaption.Name = "lblEmployeeCaption";
            this.lblEmployeeCaption.Size = new System.Drawing.Size(56, 13);
            this.lblEmployeeCaption.TabIndex = 9;
            this.lblEmployeeCaption.Text = "Employee:";
            //
            // txtEmployeeName
            //
            this.txtEmployeeName.Location = new System.Drawing.Point(450, 85);
            this.txtEmployeeName.Name = "txtEmployeeName";
            this.txtEmployeeName.ReadOnly = true;
            this.txtEmployeeName.Size = new System.Drawing.Size(170, 20);
            this.txtEmployeeName.TabIndex = 10;
            this.txtEmployeeName.TabStop = false;
            //
            // lblTotalAmountCaption
            //
            this.lblTotalAmountCaption.AutoSize = true;
            this.lblTotalAmountCaption.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblTotalAmountCaption.Location = new System.Drawing.Point(20, 123);
            this.lblTotalAmountCaption.Name = "lblTotalAmountCaption";
            this.lblTotalAmountCaption.Size = new System.Drawing.Size(88, 13);
            this.lblTotalAmountCaption.TabIndex = 11;
            this.lblTotalAmountCaption.Text = "Total Amount (USD):";
            //
            // txtTotalAmount
            //
            this.txtTotalAmount.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.txtTotalAmount.Location = new System.Drawing.Point(165, 120);
            this.txtTotalAmount.Name = "txtTotalAmount";
            this.txtTotalAmount.ReadOnly = true;
            this.txtTotalAmount.Size = new System.Drawing.Size(120, 20);
            this.txtTotalAmount.TabIndex = 12;
            this.txtTotalAmount.TabStop = false;
            //
            // lblItemsHeader
            //
            this.lblItemsHeader.AutoSize = true;
            this.lblItemsHeader.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblItemsHeader.Location = new System.Drawing.Point(20, 160);
            this.lblItemsHeader.Name = "lblItemsHeader";
            this.lblItemsHeader.Size = new System.Drawing.Size(74, 17);
            this.lblItemsHeader.TabIndex = 13;
            this.lblItemsHeader.Text = "Order Items";
            //
            // pnlButtons
            //
            this.pnlButtons.Controls.Add(this.btnClose);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlButtons.Location = new System.Drawing.Point(0, 465);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(700, 55);
            this.pnlButtons.TabIndex = 1;
            //
            // btnClose
            //
            this.btnClose.Location = new System.Drawing.Point(600, 12);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(80, 30);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // dgvItems
            //
            this.dgvItems.AllowUserToAddRows = false;
            this.dgvItems.AllowUserToDeleteRows = false;
            this.dgvItems.AllowUserToOrderColumns = false;
            this.dgvItems.AllowUserToResizeRows = false;
            this.dgvItems.AutoGenerateColumns = false;
            this.dgvItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvItems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colItemProduct,
            this.colItemQuantity,
            this.colItemUnitPrice,
            this.colItemTotal});
            this.dgvItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvItems.Location = new System.Drawing.Point(0, 190);
            this.dgvItems.MultiSelect = false;
            this.dgvItems.Name = "dgvItems";
            this.dgvItems.ReadOnly = true;
            this.dgvItems.RowHeadersWidth = 25;
            this.dgvItems.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvItems.Size = new System.Drawing.Size(700, 275);
            this.dgvItems.TabIndex = 2;
            //
            // colItemProduct
            //
            this.colItemProduct.DataPropertyName = "ProductName";
            this.colItemProduct.HeaderText = "Product";
            this.colItemProduct.Name = "colItemProduct";
            this.colItemProduct.ReadOnly = true;
            this.colItemProduct.Width = 280;
            //
            // colItemQuantity
            //
            this.colItemQuantity.DataPropertyName = "Quantity";
            this.colItemQuantity.HeaderText = "Quantity";
            this.colItemQuantity.Name = "colItemQuantity";
            this.colItemQuantity.ReadOnly = true;
            this.colItemQuantity.Width = 100;
            //
            // colItemUnitPrice
            //
            this.colItemUnitPrice.DataPropertyName = "UnitPrice";
            dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            dataGridViewCellStyle1.Format = "N2";
            this.colItemUnitPrice.DefaultCellStyle = dataGridViewCellStyle1;
            this.colItemUnitPrice.HeaderText = "Unit Price (USD)";
            this.colItemUnitPrice.Name = "colItemUnitPrice";
            this.colItemUnitPrice.ReadOnly = true;
            this.colItemUnitPrice.Width = 140;
            //
            // colItemTotal
            //
            this.colItemTotal.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colItemTotal.DataPropertyName = "Total";
            dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            dataGridViewCellStyle2.Format = "N2";
            this.colItemTotal.DefaultCellStyle = dataGridViewCellStyle2;
            this.colItemTotal.HeaderText = "Line Total (USD)";
            this.colItemTotal.Name = "colItemTotal";
            this.colItemTotal.ReadOnly = true;
            //
            // frmOrderDetails
            //
            this.AcceptButton = this.btnClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(700, 520);
            this.Controls.Add(this.dgvItems);
            this.Controls.Add(this.pnlButtons);
            this.Controls.Add(this.pnlHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(650, 500);
            this.Name = "frmOrderDetails";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Order Details";
            this.Load += new System.EventHandler(this.frmOrderDetails_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1;
        private System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2;

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblOrderIdCaption;
        private System.Windows.Forms.TextBox txtOrderId;
        private System.Windows.Forms.Label lblStatusCaption;
        private System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.Label lblOrderDateCaption;
        private System.Windows.Forms.TextBox txtOrderDate;
        private System.Windows.Forms.Label lblCustomerCaption;
        private System.Windows.Forms.TextBox txtCustomerName;
        private System.Windows.Forms.Label lblEmployeeCaption;
        private System.Windows.Forms.TextBox txtEmployeeName;
        private System.Windows.Forms.Label lblTotalAmountCaption;
        private System.Windows.Forms.TextBox txtTotalAmount;
        private System.Windows.Forms.Label lblItemsHeader;

        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.Button btnClose;

        private System.Windows.Forms.DataGridView dgvItems;
        private System.Windows.Forms.DataGridViewTextBoxColumn colItemProduct;
        private System.Windows.Forms.DataGridViewTextBoxColumn colItemQuantity;
        private System.Windows.Forms.DataGridViewTextBoxColumn colItemUnitPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn colItemTotal;
    }
}
