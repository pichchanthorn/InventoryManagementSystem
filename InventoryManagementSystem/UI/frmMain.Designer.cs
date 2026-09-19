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
            this.pnlContent = new System.Windows.Forms.Panel();
            this.pnlNav = new System.Windows.Forms.Panel();
            this.flpNav = new System.Windows.Forms.FlowLayoutPanel();
            this.btnNavDashboard = new System.Windows.Forms.Button();
            this.btnNavProducts = new System.Windows.Forms.Button();
            this.btnNavCategories = new System.Windows.Forms.Button();
            this.lblNavOverview = new System.Windows.Forms.Label();
            this.lblNavMasterData = new System.Windows.Forms.Label();
            this.lblNavInventory = new System.Windows.Forms.Label();
            this.lblNavOperations = new System.Windows.Forms.Label();
            this.lblNavReporting = new System.Windows.Forms.Label();
            this.btnNavStockIn = new System.Windows.Forms.Button();
            this.btnNavStockOut = new System.Windows.Forms.Button();
            this.btnNavOrders = new System.Windows.Forms.Button();
            this.btnNavCustomers = new System.Windows.Forms.Button();
            this.btnNavSuppliers = new System.Windows.Forms.Button();
            this.btnNavEmployees = new System.Windows.Forms.Button();
            this.btnNavReports = new System.Windows.Forms.Button();
            this.pnlNavBorder = new System.Windows.Forms.Panel();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblAppTitle = new System.Windows.Forms.Label();
            this.pnlUser = new System.Windows.Forms.Panel();
            this.pnlUserText = new System.Windows.Forms.Panel();
            this.lblUserRole = new System.Windows.Forms.Label();
            this.lblUserWelcome = new System.Windows.Forms.Label();
            this.btnLogout = new System.Windows.Forms.Button();
            this.pnlNav.SuspendLayout();
            this.flpNav.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.pnlUser.SuspendLayout();
            this.pnlUserText.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlContent
            //
            this.pnlContent.BackColor = System.Drawing.Color.White;
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(230, 68);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Size = new System.Drawing.Size(970, 652);
            this.pnlContent.TabIndex = 2;
            //
            // pnlNav
            //
            this.pnlNav.BackColor = System.Drawing.Color.FromArgb(245, 247, 250);
            this.pnlNav.Controls.Add(this.flpNav);
            this.pnlNav.Controls.Add(this.pnlNavBorder);
            this.pnlNav.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlNav.Location = new System.Drawing.Point(0, 68);
            this.pnlNav.Name = "pnlNav";
            this.pnlNav.Size = new System.Drawing.Size(230, 652);
            this.pnlNav.TabIndex = 1;
            //
            // pnlNavBorder
            //
            this.pnlNavBorder.BackColor = System.Drawing.Color.FromArgb(203, 213, 225);
            this.pnlNavBorder.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlNavBorder.Name = "pnlNavBorder";
            this.pnlNavBorder.Size = new System.Drawing.Size(1, 652);
            this.pnlNavBorder.TabIndex = 1;
            //
            // flpNav
            //
            this.flpNav.AutoScroll = true;
            this.flpNav.Controls.Add(this.lblNavOverview);
            this.flpNav.Controls.Add(this.btnNavDashboard);
            this.flpNav.Controls.Add(this.lblNavMasterData);
            this.flpNav.Controls.Add(this.btnNavCategories);
            this.flpNav.Controls.Add(this.btnNavProducts);
            this.flpNav.Controls.Add(this.btnNavCustomers);
            this.flpNav.Controls.Add(this.btnNavSuppliers);
            this.flpNav.Controls.Add(this.btnNavEmployees);
            this.flpNav.Controls.Add(this.lblNavInventory);
            this.flpNav.Controls.Add(this.btnNavStockIn);
            this.flpNav.Controls.Add(this.btnNavStockOut);
            this.flpNav.Controls.Add(this.lblNavOperations);
            this.flpNav.Controls.Add(this.btnNavOrders);
            this.flpNav.Controls.Add(this.lblNavReporting);
            this.flpNav.Controls.Add(this.btnNavReports);
            this.flpNav.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpNav.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpNav.Location = new System.Drawing.Point(0, 0);
            this.flpNav.Name = "flpNav";
            this.flpNav.Padding = new System.Windows.Forms.Padding(10, 12, 10, 8);
            this.flpNav.Size = new System.Drawing.Size(229, 652);
            this.flpNav.TabIndex = 0;
            this.flpNav.WrapContents = false;
            this.flpNav.Resize += new System.EventHandler(this.flpNav_Resize);
            //
            // btnNavDashboard
            //
            this.btnNavDashboard.Name = "btnNavDashboard";
            this.btnNavDashboard.TabIndex = 0;
            this.btnNavDashboard.Tag = "Dashboard";
            this.btnNavDashboard.Text = "Dashboard";
            this.btnNavDashboard.Click += new System.EventHandler(this.NavButton_Click);
            //
            // btnNavProducts
            //
            this.btnNavProducts.Name = "btnNavProducts";
            this.btnNavProducts.TabIndex = 1;
            this.btnNavProducts.Tag = "Products";
            this.btnNavProducts.Text = "Products";
            this.btnNavProducts.Click += new System.EventHandler(this.NavButton_Click);
            //
            // btnNavCategories
            //
            this.btnNavCategories.Name = "btnNavCategories";
            this.btnNavCategories.TabIndex = 2;
            this.btnNavCategories.Tag = "Categories";
            this.btnNavCategories.Text = "Categories";
            this.btnNavCategories.Click += new System.EventHandler(this.NavButton_Click);
            //
            // lblNavOverview
            //
            this.lblNavOverview.Name = "lblNavOverview";
            this.lblNavOverview.Text = "OVERVIEW";
            //
            // lblNavMasterData
            //
            this.lblNavMasterData.Name = "lblNavMasterData";
            this.lblNavMasterData.Text = "MASTER DATA";
            //
            // lblNavInventory
            //
            this.lblNavInventory.Name = "lblNavInventory";
            this.lblNavInventory.Text = "INVENTORY";
            //
            // lblNavOperations
            //
            this.lblNavOperations.Name = "lblNavOperations";
            this.lblNavOperations.Text = "OPERATIONS";
            //
            // lblNavReporting
            //
            this.lblNavReporting.Name = "lblNavReporting";
            this.lblNavReporting.Text = "REPORTING";
            //
            // btnNavStockIn
            //
            this.btnNavStockIn.Name = "btnNavStockIn";
            this.btnNavStockIn.TabIndex = 3;
            this.btnNavStockIn.Tag = "Stock In";
            this.btnNavStockIn.Text = "Stock In";
            this.btnNavStockIn.Click += new System.EventHandler(this.NavButton_Click);
            //
            // btnNavStockOut
            //
            this.btnNavStockOut.Name = "btnNavStockOut";
            this.btnNavStockOut.TabIndex = 4;
            this.btnNavStockOut.Tag = "Stock Out";
            this.btnNavStockOut.Text = "Stock Out";
            this.btnNavStockOut.Click += new System.EventHandler(this.NavButton_Click);
            //
            // btnNavOrders
            //
            this.btnNavOrders.Name = "btnNavOrders";
            this.btnNavOrders.TabIndex = 5;
            this.btnNavOrders.Tag = "Orders";
            this.btnNavOrders.Text = "Orders";
            this.btnNavOrders.Click += new System.EventHandler(this.NavButton_Click);
            //
            // btnNavCustomers
            //
            this.btnNavCustomers.Name = "btnNavCustomers";
            this.btnNavCustomers.TabIndex = 6;
            this.btnNavCustomers.Tag = "Customers";
            this.btnNavCustomers.Text = "Customers";
            this.btnNavCustomers.Click += new System.EventHandler(this.NavButton_Click);
            //
            // btnNavSuppliers
            //
            this.btnNavSuppliers.Name = "btnNavSuppliers";
            this.btnNavSuppliers.TabIndex = 7;
            this.btnNavSuppliers.Tag = "Suppliers";
            this.btnNavSuppliers.Text = "Suppliers";
            this.btnNavSuppliers.Click += new System.EventHandler(this.NavButton_Click);
            //
            // btnNavEmployees
            //
            this.btnNavEmployees.Name = "btnNavEmployees";
            this.btnNavEmployees.TabIndex = 8;
            this.btnNavEmployees.Tag = "Employees";
            this.btnNavEmployees.Text = "Employees";
            this.btnNavEmployees.Click += new System.EventHandler(this.NavButton_Click);
            //
            // btnNavReports
            //
            this.btnNavReports.Name = "btnNavReports";
            this.btnNavReports.TabIndex = 9;
            this.btnNavReports.Tag = "Reports";
            this.btnNavReports.Text = "Reports";
            this.btnNavReports.Click += new System.EventHandler(this.NavButton_Click);
            //
            // pnlHeader
            //
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(33, 47, 61);
            this.pnlHeader.Controls.Add(this.lblAppTitle);
            this.pnlHeader.Controls.Add(this.pnlUser);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Padding = new System.Windows.Forms.Padding(24, 0, 0, 0);
            this.pnlHeader.Size = new System.Drawing.Size(1200, 68);
            this.pnlHeader.TabIndex = 0;
            //
            // lblAppTitle
            //
            this.lblAppTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAppTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblAppTitle.ForeColor = System.Drawing.Color.White;
            this.lblAppTitle.Name = "lblAppTitle";
            this.lblAppTitle.TabIndex = 0;
            this.lblAppTitle.Text = "Inventory Management System";
            this.lblAppTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlUser
            //
            this.pnlUser.Controls.Add(this.pnlUserText);
            this.pnlUser.Controls.Add(this.btnLogout);
            this.pnlUser.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlUser.Name = "pnlUser";
            this.pnlUser.Padding = new System.Windows.Forms.Padding(0, 15, 24, 15);
            this.pnlUser.Size = new System.Drawing.Size(420, 68);
            this.pnlUser.TabIndex = 1;
            //
            // btnLogout
            //
            this.btnLogout.BackColor = System.Drawing.Color.FromArgb(60, 76, 92);
            this.btnLogout.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnLogout.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.btnLogout.FlatAppearance.BorderSize = 1;
            this.btnLogout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogout.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.btnLogout.ForeColor = System.Drawing.Color.White;
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(96, 38);
            this.btnLogout.TabIndex = 1;
            this.btnLogout.Text = "Logout";
            this.btnLogout.UseVisualStyleBackColor = false;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            //
            // pnlUserText
            //
            this.pnlUserText.Controls.Add(this.lblUserRole);
            this.pnlUserText.Controls.Add(this.lblUserWelcome);
            this.pnlUserText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlUserText.Name = "pnlUserText";
            this.pnlUserText.Padding = new System.Windows.Forms.Padding(0, 0, 16, 0);
            this.pnlUserText.TabIndex = 0;
            //
            // lblUserWelcome
            //
            this.lblUserWelcome.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblUserWelcome.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.lblUserWelcome.ForeColor = System.Drawing.Color.White;
            this.lblUserWelcome.Name = "lblUserWelcome";
            this.lblUserWelcome.Size = new System.Drawing.Size(280, 21);
            this.lblUserWelcome.TabIndex = 0;
            this.lblUserWelcome.Text = "Welcome,";
            this.lblUserWelcome.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblUserRole
            //
            this.lblUserRole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblUserRole.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblUserRole.ForeColor = System.Drawing.Color.FromArgb(203, 213, 225);
            this.lblUserRole.Name = "lblUserRole";
            this.lblUserRole.TabIndex = 1;
            this.lblUserRole.Text = "Role:";
            this.lblUserRole.TextAlign = System.Drawing.ContentAlignment.TopRight;
            //
            // frmMain
            //
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlNav);
            this.Controls.Add(this.pnlHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.MinimumSize = new System.Drawing.Size(1100, 640);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Inventory Management System";
            this.pnlNav.ResumeLayout(false);
            this.flpNav.ResumeLayout(false);
            this.pnlHeader.ResumeLayout(false);
            this.pnlUser.ResumeLayout(false);
            this.pnlUserText.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblAppTitle;
        private System.Windows.Forms.Panel pnlUser;
        private System.Windows.Forms.Panel pnlUserText;
        private System.Windows.Forms.Label lblUserWelcome;
        private System.Windows.Forms.Label lblUserRole;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.Panel pnlNav;
        private System.Windows.Forms.Panel pnlNavBorder;
        private System.Windows.Forms.FlowLayoutPanel flpNav;
        private System.Windows.Forms.Button btnNavDashboard;
        private System.Windows.Forms.Button btnNavProducts;
        private System.Windows.Forms.Button btnNavCategories;
        private System.Windows.Forms.Label lblNavOverview;
        private System.Windows.Forms.Label lblNavMasterData;
        private System.Windows.Forms.Label lblNavInventory;
        private System.Windows.Forms.Label lblNavOperations;
        private System.Windows.Forms.Label lblNavReporting;
        private System.Windows.Forms.Button btnNavStockIn;
        private System.Windows.Forms.Button btnNavStockOut;
        private System.Windows.Forms.Button btnNavOrders;
        private System.Windows.Forms.Button btnNavCustomers;
        private System.Windows.Forms.Button btnNavSuppliers;
        private System.Windows.Forms.Button btnNavEmployees;
        private System.Windows.Forms.Button btnNavReports;
        private System.Windows.Forms.Panel pnlContent;
    }
}
