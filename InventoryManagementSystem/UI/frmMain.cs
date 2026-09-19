using System;
using System.Drawing;
using System.Windows.Forms;
using InventoryManagementSystem.Entity;

namespace InventoryManagementSystem.UI
{
    /// <summary>
    /// Application shell: header + sidebar + one content area. Every module is a UserControl page shown inside
    /// the content area; navigation never opens another top-level window.
    /// </summary>
    public partial class frmMain : Form
    {
        private static readonly Color NavDefaultBackColor = Color.FromArgb(245, 247, 250);
        private static readonly Color NavHoverBackColor = Color.FromArgb(226, 232, 240);

        private const string DashboardKey = "Dashboard";

        private Button[] _navButtons;
        private Label[] _navHeadings;
        private Button _selectedNavButton;
        private ucDashboard _dashboard;      // created once, refreshed every time it is shown
        private UserControl _currentPage;
        private string _currentKey;

        public UserEntity AuthenticatedUser { get; private set; }
        public bool LogoutRequested { get; private set; }

        /// <summary>The page currently shown in the content area (never null once a module has been selected).</summary>
        public UserControl CurrentPage { get { return _currentPage; } }

        /// <summary>Navigation key of the current page, e.g. "Stock In".</summary>
        public string CurrentModule { get { return _currentKey; } }

        public frmMain()
        {
            InitializeComponent();
            InitializeNavigation();
            InitializeDashboard();
        }

        public frmMain(UserEntity authenticatedUser) : this()
        {
            AuthenticatedUser = authenticatedUser;

            if (AuthenticatedUser != null)
            {
                lblUserWelcome.Text = "Welcome, " + AuthenticatedUser.FullName;
                lblUserRole.Text = "Role: " + AuthenticatedUser.Role;
            }

            SelectModule(btnNavDashboard);
        }

        // ---- navigation ------------------------------------------------------------------------

        private void InitializeNavigation()
        {
            _navButtons = new[]
            {
                btnNavDashboard, btnNavCategories, btnNavProducts, btnNavCustomers, btnNavSuppliers, btnNavEmployees,
                btnNavStockIn, btnNavStockOut, btnNavOrders, btnNavReports
            };
            _navHeadings = new[] { lblNavOverview, lblNavMasterData, lblNavInventory, lblNavOperations, lblNavReporting };

            foreach (Button navButton in _navButtons)
            {
                navButton.FlatStyle = FlatStyle.Flat;
                navButton.FlatAppearance.BorderSize = 0;
                navButton.UseVisualStyleBackColor = false;
                navButton.BackColor = NavDefaultBackColor;
                navButton.ForeColor = Theme.TextDark;
                navButton.Font = Theme.BaseFont;
                navButton.Cursor = Cursors.Hand;
                navButton.TextAlign = ContentAlignment.MiddleLeft;
                navButton.Padding = new Padding(26, 0, 0, 0);
                navButton.Margin = new Padding(0, 1, 0, 1);
                navButton.Height = 36;

                Button captured = navButton;
                navButton.MouseEnter += (s, e) => { if (captured != _selectedNavButton) captured.BackColor = NavHoverBackColor; };
                navButton.MouseLeave += (s, e) => { if (captured != _selectedNavButton) captured.BackColor = NavDefaultBackColor; };
            }

            // Group headings: OVERVIEW / MASTER DATA / INVENTORY / OPERATIONS / REPORTING
            foreach (Label heading in _navHeadings)
            {
                heading.AutoSize = false;
                heading.Height = 26;
                heading.Margin = new Padding(0, heading == lblNavOverview ? 0 : 8, 0, 0);
                heading.Padding = new Padding(14, 0, 0, 0);
                heading.Font = new Font("Segoe UI Semibold", 8.5F);
                heading.ForeColor = Theme.TextMuted;
                heading.TextAlign = ContentAlignment.BottomLeft;
            }

            btnLogout.MouseEnter += (s, e) => btnLogout.BackColor = Color.FromArgb(80, 96, 112);
            btnLogout.MouseLeave += (s, e) => btnLogout.BackColor = Color.FromArgb(60, 76, 92);

            ResizeNavButtons();
        }

        private void flpNav_Resize(object sender, EventArgs e)
        {
            ResizeNavButtons();
        }

        private void ResizeNavButtons()
        {
            if (_navButtons == null)
                return;

            int width = flpNav.ClientSize.Width - flpNav.Padding.Horizontal;
            if (width < 100)
                return;

            foreach (Button navButton in _navButtons)
                navButton.Width = width;
            foreach (Label heading in _navHeadings)
                heading.Width = width;
        }

        private void NavButton_Click(object sender, EventArgs e)
        {
            SelectModule((Button)sender);
        }

        private void SelectModule(Button selectedButton)
        {
            _selectedNavButton = selectedButton;
            foreach (Button navButton in _navButtons)
            {
                bool isSelected = navButton == selectedButton;
                navButton.BackColor = isSelected ? Theme.Primary : NavDefaultBackColor;
                navButton.ForeColor = isSelected ? Color.White : Theme.TextDark;
                navButton.Font = isSelected ? Theme.SemiboldFont : Theme.BaseFont;
            }

            ShowModule((string)selectedButton.Tag);
        }

        // ---- content area ----------------------------------------------------------------------

        private void InitializeDashboard()
        {
            _dashboard = new ucDashboard();
        }

        private void ShowModule(string key)
        {
            if (key == DashboardKey)
            {
                ShowPage(_dashboard, key);
                _dashboard.LoadData();   // always re-query when the Dashboard is (re)selected
                return;
            }

            // Selecting the page that is already showing keeps it (and anything half-typed) as is.
            if (key == _currentKey && _currentPage != null && !_currentPage.IsDisposed)
                return;

            ShowPage(CreatePage(key), key);
        }

        private static UserControl CreatePage(string key)
        {
            switch (key)
            {
                case "Categories": return new ucCategories();
                case "Products": return new ucProducts();
                case "Customers": return new ucCustomers();
                case "Suppliers": return new ucSuppliers();
                case "Employees": return new ucEmployees();
                case "Stock In": return new ucStockIn();
                case "Stock Out": return new ucStockOut();
                case "Orders": return new ucOrders();
                case "Reports": return new ucReports();
                default: throw new ArgumentException("Unknown module: " + key);
            }
        }

        /// <summary>Replaces whatever is in the content area with <paramref name="page"/>; exactly one page is ever hosted.</summary>
        private void ShowPage(UserControl page, string key)
        {
            SuspendLayout();
            pnlContent.SuspendLayout();

            UserControl previous = _currentPage;
            if (previous != null && previous != page)
            {
                pnlContent.Controls.Remove(previous);
                if (previous != _dashboard)
                    previous.Dispose();      // pages are cheap to rebuild and always start with fresh data
            }

            if (page.Parent != pnlContent)
            {
                page.Dock = DockStyle.Fill;
                pnlContent.Controls.Add(page);
            }
            page.Visible = true;
            page.BringToFront();

            _currentPage = page;
            _currentKey = key;
            Text = "Inventory Management System - " + key;

            pnlContent.ResumeLayout(true);
            ResumeLayout(true);
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            LogoutRequested = true;
            Close();
        }
    }
}
