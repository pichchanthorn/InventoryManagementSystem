namespace InventoryManagementSystem.UI
{
    partial class frmMain
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
            this.lblAppTitle = new System.Windows.Forms.Label();
            this.lblUserWelcome = new System.Windows.Forms.Label();
            this.lblUserRole = new System.Windows.Forms.Label();
            this.btnLogout = new System.Windows.Forms.Button();
            this.pnlNav = new System.Windows.Forms.Panel();
            this.flpNav = new System.Windows.Forms.FlowLayoutPanel();
            this.btnNavDashboard = new System.Windows.Forms.Button();
            this.btnNavCategories = new System.Windows.Forms.Button();
            this.btnNavProducts = new System.Windows.Forms.Button();
            this.btnNavCustomers = new System.Windows.Forms.Button();
            this.btnNavSuppliers = new System.Windows.Forms.Button();
            this.btnNavEmployees = new System.Windows.Forms.Button();
            this.btnNavStockIn = new System.Windows.Forms.Button();
            this.btnNavStockOut = new System.Windows.Forms.Button();
            this.btnNavOrders = new System.Windows.Forms.Button();
            this.btnNavReports = new System.Windows.Forms.Button();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.lblPlaceholder = new System.Windows.Forms.Label();
            this.pnlWelcome = new System.Windows.Forms.Panel();
            this.lblWelcomeStatus = new System.Windows.Forms.Label();
            this.lblWelcomeRole = new System.Windows.Forms.Label();
            this.lblWelcomeGreeting = new System.Windows.Forms.Label();
            this.lblWelcomeAppName = new System.Windows.Forms.Label();
            this.pnlHeader.SuspendLayout();
            this.pnlNav.SuspendLayout();
            this.flpNav.SuspendLayout();
            this.pnlContent.SuspendLayout();
            this.pnlWelcome.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlHeader
            //
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(33, 47, 61);
            this.pnlHeader.Controls.Add(this.lblAppTitle);
            this.pnlHeader.Controls.Add(this.lblUserWelcome);
            this.pnlHeader.Controls.Add(this.lblUserRole);
            this.pnlHeader.Controls.Add(this.btnLogout);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1000, 64);
            this.pnlHeader.TabIndex = 0;
            //
            // lblAppTitle
            //
            this.lblAppTitle.AutoSize = true;
            this.lblAppTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblAppTitle.ForeColor = System.Drawing.Color.White;
            this.lblAppTitle.Location = new System.Drawing.Point(20, 16);
            this.lblAppTitle.Name = "lblAppTitle";
            this.lblAppTitle.Size = new System.Drawing.Size(320, 25);
            this.lblAppTitle.TabIndex = 0;
            this.lblAppTitle.Text = "Inventory Management System";
            //
            // lblUserWelcome
            //
            this.lblUserWelcome.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblUserWelcome.AutoSize = true;
            this.lblUserWelcome.ForeColor = System.Drawing.Color.White;
            this.lblUserWelcome.Location = new System.Drawing.Point(650, 14);
            this.lblUserWelcome.Name = "lblUserWelcome";
            this.lblUserWelcome.Size = new System.Drawing.Size(63, 13);
            this.lblUserWelcome.TabIndex = 1;
            this.lblUserWelcome.Text = "Welcome,";
            this.lblUserWelcome.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblUserRole
            //
            this.lblUserRole.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblUserRole.AutoSize = true;
            this.lblUserRole.ForeColor = System.Drawing.Color.Gainsboro;
            this.lblUserRole.Location = new System.Drawing.Point(650, 36);
            this.lblUserRole.Name = "lblUserRole";
            this.lblUserRole.Size = new System.Drawing.Size(34, 13);
            this.lblUserRole.TabIndex = 2;
            this.lblUserRole.Text = "Role:";
            this.lblUserRole.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // btnLogout
            //
            this.btnLogout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLogout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogout.ForeColor = System.Drawing.Color.White;
            this.btnLogout.Location = new System.Drawing.Point(890, 16);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(90, 32);
            this.btnLogout.TabIndex = 3;
            this.btnLogout.Text = "Logout";
            this.btnLogout.UseVisualStyleBackColor = false;
            this.btnLogout.BackColor = System.Drawing.Color.FromArgb(60, 76, 92);
            this.btnLogout.FlatAppearance.BorderSize = 0;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            //
            // pnlNav
            //
            this.pnlNav.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
            this.pnlNav.Controls.Add(this.flpNav);
            this.pnlNav.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlNav.Location = new System.Drawing.Point(0, 64);
            this.pnlNav.Name = "pnlNav";
            this.pnlNav.Size = new System.Drawing.Size(190, 556);
            this.pnlNav.TabIndex = 1;
            //
            // flpNav
            //
            this.flpNav.AutoScroll = true;
            this.flpNav.Controls.Add(this.btnNavDashboard);
            this.flpNav.Controls.Add(this.btnNavCategories);
            this.flpNav.Controls.Add(this.btnNavProducts);
            this.flpNav.Controls.Add(this.btnNavCustomers);
            this.flpNav.Controls.Add(this.btnNavSuppliers);
            this.flpNav.Controls.Add(this.btnNavEmployees);
            this.flpNav.Controls.Add(this.btnNavStockIn);
            this.flpNav.Controls.Add(this.btnNavStockOut);
            this.flpNav.Controls.Add(this.btnNavOrders);
            this.flpNav.Controls.Add(this.btnNavReports);
            this.flpNav.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpNav.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpNav.Location = new System.Drawing.Point(0, 0);
            this.flpNav.Name = "flpNav";
            this.flpNav.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.flpNav.Size = new System.Drawing.Size(190, 556);
            this.flpNav.TabIndex = 0;
            this.flpNav.WrapContents = false;
            //
            // btnNavDashboard
            //
            this.btnNavDashboard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavDashboard.Location = new System.Drawing.Point(3, 3);
            this.btnNavDashboard.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.btnNavDashboard.Name = "btnNavDashboard";
            this.btnNavDashboard.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnNavDashboard.Size = new System.Drawing.Size(180, 42);
            this.btnNavDashboard.TabIndex = 0;
            this.btnNavDashboard.Tag = "Dashboard";
            this.btnNavDashboard.Text = "Dashboard";
            this.btnNavDashboard.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNavDashboard.UseVisualStyleBackColor = true;
            this.btnNavDashboard.Click += new System.EventHandler(this.NavButton_Click);
            //
            // btnNavCategories
            //
            this.btnNavCategories.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavCategories.Location = new System.Drawing.Point(3, 48);
            this.btnNavCategories.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.btnNavCategories.Name = "btnNavCategories";
            this.btnNavCategories.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnNavCategories.Size = new System.Drawing.Size(180, 42);
            this.btnNavCategories.TabIndex = 1;
            this.btnNavCategories.Tag = "Categories";
            this.btnNavCategories.Text = "Categories";
            this.btnNavCategories.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNavCategories.UseVisualStyleBackColor = true;
            this.btnNavCategories.Click += new System.EventHandler(this.NavButton_Click);
            //
            // btnNavProducts
            //
            this.btnNavProducts.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavProducts.Location = new System.Drawing.Point(3, 93);
            this.btnNavProducts.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.btnNavProducts.Name = "btnNavProducts";
            this.btnNavProducts.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnNavProducts.Size = new System.Drawing.Size(180, 42);
            this.btnNavProducts.TabIndex = 2;
            this.btnNavProducts.Tag = "Products";
            this.btnNavProducts.Text = "Products";
            this.btnNavProducts.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNavProducts.UseVisualStyleBackColor = true;
            this.btnNavProducts.Click += new System.EventHandler(this.NavButton_Click);
            //
            // btnNavCustomers
            //
            this.btnNavCustomers.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavCustomers.Location = new System.Drawing.Point(3, 138);
            this.btnNavCustomers.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.btnNavCustomers.Name = "btnNavCustomers";
            this.btnNavCustomers.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnNavCustomers.Size = new System.Drawing.Size(180, 42);
            this.btnNavCustomers.TabIndex = 3;
            this.btnNavCustomers.Tag = "Customers";
            this.btnNavCustomers.Text = "Customers";
            this.btnNavCustomers.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNavCustomers.UseVisualStyleBackColor = true;
            this.btnNavCustomers.Click += new System.EventHandler(this.NavButton_Click);
            //
            // btnNavSuppliers
            //
            this.btnNavSuppliers.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavSuppliers.Location = new System.Drawing.Point(3, 183);
            this.btnNavSuppliers.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.btnNavSuppliers.Name = "btnNavSuppliers";
            this.btnNavSuppliers.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnNavSuppliers.Size = new System.Drawing.Size(180, 42);
            this.btnNavSuppliers.TabIndex = 4;
            this.btnNavSuppliers.Tag = "Suppliers";
            this.btnNavSuppliers.Text = "Suppliers";
            this.btnNavSuppliers.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNavSuppliers.UseVisualStyleBackColor = true;
            this.btnNavSuppliers.Click += new System.EventHandler(this.NavButton_Click);
            //
            // btnNavEmployees
            //
            this.btnNavEmployees.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavEmployees.Location = new System.Drawing.Point(3, 228);
            this.btnNavEmployees.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.btnNavEmployees.Name = "btnNavEmployees";
            this.btnNavEmployees.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnNavEmployees.Size = new System.Drawing.Size(180, 42);
            this.btnNavEmployees.TabIndex = 5;
            this.btnNavEmployees.Tag = "Employees";
            this.btnNavEmployees.Text = "Employees";
            this.btnNavEmployees.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNavEmployees.UseVisualStyleBackColor = true;
            this.btnNavEmployees.Click += new System.EventHandler(this.NavButton_Click);
            //
            // btnNavStockIn
            //
            this.btnNavStockIn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavStockIn.Location = new System.Drawing.Point(3, 273);
            this.btnNavStockIn.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.btnNavStockIn.Name = "btnNavStockIn";
            this.btnNavStockIn.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnNavStockIn.Size = new System.Drawing.Size(180, 42);
            this.btnNavStockIn.TabIndex = 6;
            this.btnNavStockIn.Tag = "Stock In";
            this.btnNavStockIn.Text = "Stock In";
            this.btnNavStockIn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNavStockIn.UseVisualStyleBackColor = true;
            this.btnNavStockIn.Click += new System.EventHandler(this.NavButton_Click);
            //
            // btnNavStockOut
            //
            this.btnNavStockOut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavStockOut.Location = new System.Drawing.Point(3, 318);
            this.btnNavStockOut.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.btnNavStockOut.Name = "btnNavStockOut";
            this.btnNavStockOut.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnNavStockOut.Size = new System.Drawing.Size(180, 42);
            this.btnNavStockOut.TabIndex = 7;
            this.btnNavStockOut.Tag = "Stock Out";
            this.btnNavStockOut.Text = "Stock Out";
            this.btnNavStockOut.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNavStockOut.UseVisualStyleBackColor = true;
            this.btnNavStockOut.Click += new System.EventHandler(this.NavButton_Click);
            //
            // btnNavOrders
            //
            this.btnNavOrders.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavOrders.Location = new System.Drawing.Point(3, 363);
            this.btnNavOrders.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.btnNavOrders.Name = "btnNavOrders";
            this.btnNavOrders.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnNavOrders.Size = new System.Drawing.Size(180, 42);
            this.btnNavOrders.TabIndex = 8;
            this.btnNavOrders.Tag = "Orders";
            this.btnNavOrders.Text = "Orders";
            this.btnNavOrders.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNavOrders.UseVisualStyleBackColor = true;
            this.btnNavOrders.Click += new System.EventHandler(this.NavButton_Click);
            //
            // btnNavReports
            //
            this.btnNavReports.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavReports.Location = new System.Drawing.Point(3, 408);
            this.btnNavReports.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.btnNavReports.Name = "btnNavReports";
            this.btnNavReports.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnNavReports.Size = new System.Drawing.Size(180, 42);
            this.btnNavReports.TabIndex = 9;
            this.btnNavReports.Tag = "Reports";
            this.btnNavReports.Text = "Reports";
            this.btnNavReports.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNavReports.UseVisualStyleBackColor = true;
            this.btnNavReports.Click += new System.EventHandler(this.NavButton_Click);
            //
            // pnlContent
            //
            this.pnlContent.BackColor = System.Drawing.Color.White;
            this.pnlContent.Controls.Add(this.lblPlaceholder);
            this.pnlContent.Controls.Add(this.pnlWelcome);
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(190, 64);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Padding = new System.Windows.Forms.Padding(30);
            this.pnlContent.Size = new System.Drawing.Size(810, 556);
            this.pnlContent.TabIndex = 2;
            //
            // lblPlaceholder
            //
            this.lblPlaceholder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPlaceholder.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblPlaceholder.ForeColor = System.Drawing.Color.DimGray;
            this.lblPlaceholder.Location = new System.Drawing.Point(30, 30);
            this.lblPlaceholder.Name = "lblPlaceholder";
            this.lblPlaceholder.Size = new System.Drawing.Size(750, 496);
            this.lblPlaceholder.TabIndex = 1;
            this.lblPlaceholder.Text = "This module will be implemented in a future phase.";
            this.lblPlaceholder.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblPlaceholder.Visible = false;
            //
            // pnlWelcome
            //
            this.pnlWelcome.Controls.Add(this.lblWelcomeStatus);
            this.pnlWelcome.Controls.Add(this.lblWelcomeRole);
            this.pnlWelcome.Controls.Add(this.lblWelcomeGreeting);
            this.pnlWelcome.Controls.Add(this.lblWelcomeAppName);
            this.pnlWelcome.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlWelcome.Location = new System.Drawing.Point(30, 30);
            this.pnlWelcome.Name = "pnlWelcome";
            this.pnlWelcome.Size = new System.Drawing.Size(750, 496);
            this.pnlWelcome.TabIndex = 0;
            //
            // lblWelcomeStatus
            //
            this.lblWelcomeStatus.AutoSize = true;
            this.lblWelcomeStatus.ForeColor = System.Drawing.Color.DimGray;
            this.lblWelcomeStatus.Location = new System.Drawing.Point(3, 130);
            this.lblWelcomeStatus.Name = "lblWelcomeStatus";
            this.lblWelcomeStatus.Size = new System.Drawing.Size(300, 13);
            this.lblWelcomeStatus.TabIndex = 3;
            this.lblWelcomeStatus.Text = "Status: Login and navigation foundation active.";
            //
            // lblWelcomeRole
            //
            this.lblWelcomeRole.AutoSize = true;
            this.lblWelcomeRole.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblWelcomeRole.Location = new System.Drawing.Point(3, 100);
            this.lblWelcomeRole.Name = "lblWelcomeRole";
            this.lblWelcomeRole.Size = new System.Drawing.Size(38, 19);
            this.lblWelcomeRole.TabIndex = 2;
            this.lblWelcomeRole.Text = "Role:";
            //
            // lblWelcomeGreeting
            //
            this.lblWelcomeGreeting.AutoSize = true;
            this.lblWelcomeGreeting.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblWelcomeGreeting.Location = new System.Drawing.Point(3, 70);
            this.lblWelcomeGreeting.Name = "lblWelcomeGreeting";
            this.lblWelcomeGreeting.Size = new System.Drawing.Size(75, 19);
            this.lblWelcomeGreeting.TabIndex = 1;
            this.lblWelcomeGreeting.Text = "Welcome,";
            //
            // lblWelcomeAppName
            //
            this.lblWelcomeAppName.AutoSize = true;
            this.lblWelcomeAppName.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblWelcomeAppName.Location = new System.Drawing.Point(0, 20);
            this.lblWelcomeAppName.Name = "lblWelcomeAppName";
            this.lblWelcomeAppName.Size = new System.Drawing.Size(320, 30);
            this.lblWelcomeAppName.TabIndex = 0;
            this.lblWelcomeAppName.Text = "Inventory Management System";
            //
            // frmMain
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 620);
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlNav);
            this.Controls.Add(this.pnlHeader);
            this.MinimumSize = new System.Drawing.Size(800, 500);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Inventory Management System";
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlNav.ResumeLayout(false);
            this.flpNav.ResumeLayout(false);
            this.pnlContent.ResumeLayout(false);
            this.pnlWelcome.ResumeLayout(false);
            this.pnlWelcome.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblAppTitle;
        private System.Windows.Forms.Label lblUserWelcome;
        private System.Windows.Forms.Label lblUserRole;
        private System.Windows.Forms.Button btnLogout;

        private System.Windows.Forms.Panel pnlNav;
        private System.Windows.Forms.FlowLayoutPanel flpNav;
        private System.Windows.Forms.Button btnNavDashboard;
        private System.Windows.Forms.Button btnNavCategories;
        private System.Windows.Forms.Button btnNavProducts;
        private System.Windows.Forms.Button btnNavCustomers;
        private System.Windows.Forms.Button btnNavSuppliers;
        private System.Windows.Forms.Button btnNavEmployees;
        private System.Windows.Forms.Button btnNavStockIn;
        private System.Windows.Forms.Button btnNavStockOut;
        private System.Windows.Forms.Button btnNavOrders;
        private System.Windows.Forms.Button btnNavReports;

        private System.Windows.Forms.Panel pnlContent;
        private System.Windows.Forms.Label lblPlaceholder;
        private System.Windows.Forms.Panel pnlWelcome;
        private System.Windows.Forms.Label lblWelcomeStatus;
        private System.Windows.Forms.Label lblWelcomeRole;
        private System.Windows.Forms.Label lblWelcomeGreeting;
        private System.Windows.Forms.Label lblWelcomeAppName;
    }
}
