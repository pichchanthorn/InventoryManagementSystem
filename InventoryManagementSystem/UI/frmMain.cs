using System;
using System.Drawing;
using System.Windows.Forms;
using InventoryManagementSystem.Entity;

namespace InventoryManagementSystem.UI
{
    public partial class frmMain : Form
    {
        private static readonly Color NavDefaultBackColor = Color.FromArgb(240, 240, 240);
        private static readonly Color NavSelectedBackColor = Color.FromArgb(0, 120, 215);

        private Button[] _navButtons;
        private frmCategories _frmCategories;
        private frmProducts _frmProducts;
        private frmCustomers _frmCustomers;
        private frmSuppliers _frmSuppliers;
        private frmEmployees _frmEmployees;

        public UserEntity AuthenticatedUser { get; private set; }
        public bool LogoutRequested { get; private set; }

        public frmMain()
        {
            InitializeComponent();
            InitializeNavigation();
        }

        public frmMain(UserEntity authenticatedUser) : this()
        {
            AuthenticatedUser = authenticatedUser;

            if (AuthenticatedUser != null)
            {
                lblUserWelcome.Text = "Welcome, " + AuthenticatedUser.FullName;
                lblUserRole.Text = "Role: " + AuthenticatedUser.Role;
                lblWelcomeGreeting.Text = "Welcome, " + AuthenticatedUser.FullName;
                lblWelcomeRole.Text = "Role: " + AuthenticatedUser.Role;
            }

            SelectModule(btnNavDashboard);
        }

        private void InitializeNavigation()
        {
            _navButtons = new[]
            {
                btnNavDashboard,
                btnNavCategories,
                btnNavProducts,
                btnNavCustomers,
                btnNavSuppliers,
                btnNavEmployees,
                btnNavStockIn,
                btnNavStockOut,
                btnNavOrders,
                btnNavReports
            };

            foreach (Button navButton in _navButtons)
            {
                navButton.FlatAppearance.BorderSize = 0;
                navButton.BackColor = NavDefaultBackColor;
            }
        }

        private void NavButton_Click(object sender, EventArgs e)
        {
            SelectModule((Button)sender);
        }

        private void SelectModule(Button selectedButton)
        {
            foreach (Button navButton in _navButtons)
            {
                bool isSelected = navButton == selectedButton;
                navButton.BackColor = isSelected ? NavSelectedBackColor : NavDefaultBackColor;
                navButton.ForeColor = isSelected ? Color.White : Color.Black;
            }

            string moduleName = (string)selectedButton.Tag;

            if (moduleName == "Dashboard")
            {
                pnlWelcome.Visible = true;
                lblPlaceholder.Visible = false;
            }
            else if (moduleName == "Categories")
            {
                pnlWelcome.Visible = false;
                lblPlaceholder.Text = "Categories — the Category Management window is open. Close it to return here.";
                lblPlaceholder.Visible = true;
                OpenCategoriesModule();
            }
            else if (moduleName == "Products")
            {
                pnlWelcome.Visible = false;
                lblPlaceholder.Text = "Products — the Product Management window is open. Close it to return here.";
                lblPlaceholder.Visible = true;
                OpenProductsModule();
            }
            else if (moduleName == "Customers")
            {
                pnlWelcome.Visible = false;
                lblPlaceholder.Text = "Customers — the Customer Management window is open. Close it to return here.";
                lblPlaceholder.Visible = true;
                OpenCustomersModule();
            }
            else if (moduleName == "Suppliers")
            {
                pnlWelcome.Visible = false;
                lblPlaceholder.Text = "Suppliers — the Supplier Management window is open. Close it to return here.";
                lblPlaceholder.Visible = true;
                OpenSuppliersModule();
            }
            else if (moduleName == "Employees")
            {
                pnlWelcome.Visible = false;
                lblPlaceholder.Text = "Employees — the Employee Management window is open. Close it to return here.";
                lblPlaceholder.Visible = true;
                OpenEmployeesModule();
            }
            else
            {
                lblPlaceholder.Text = moduleName + " — This module will be implemented in a future phase.";
                pnlWelcome.Visible = false;
                lblPlaceholder.Visible = true;
            }
        }

        private void OpenCategoriesModule()
        {
            if (_frmCategories == null || _frmCategories.IsDisposed)
            {
                _frmCategories = new frmCategories();
                _frmCategories.FormClosed += (sender, e) => { _frmCategories = null; };
                _frmCategories.Show();
            }
            else
            {
                if (_frmCategories.WindowState == FormWindowState.Minimized)
                    _frmCategories.WindowState = FormWindowState.Normal;

                _frmCategories.Activate();
                _frmCategories.BringToFront();
            }
        }

        private void OpenProductsModule()
        {
            if (_frmProducts == null || _frmProducts.IsDisposed)
            {
                _frmProducts = new frmProducts();
                _frmProducts.FormClosed += (sender, e) => { _frmProducts = null; };
                _frmProducts.Show();
            }
            else
            {
                if (_frmProducts.WindowState == FormWindowState.Minimized)
                    _frmProducts.WindowState = FormWindowState.Normal;

                _frmProducts.Activate();
                _frmProducts.BringToFront();
            }
        }

        private void OpenCustomersModule()
        {
            if (_frmCustomers == null || _frmCustomers.IsDisposed)
            {
                _frmCustomers = new frmCustomers();
                _frmCustomers.FormClosed += (sender, e) => { _frmCustomers = null; };
                _frmCustomers.Show();
            }
            else
            {
                if (_frmCustomers.WindowState == FormWindowState.Minimized)
                    _frmCustomers.WindowState = FormWindowState.Normal;

                _frmCustomers.Activate();
                _frmCustomers.BringToFront();
            }
        }

        private void OpenSuppliersModule()
        {
            if (_frmSuppliers == null || _frmSuppliers.IsDisposed)
            {
                _frmSuppliers = new frmSuppliers();
                _frmSuppliers.FormClosed += (sender, e) => { _frmSuppliers = null; };
                _frmSuppliers.Show();
            }
            else
            {
                if (_frmSuppliers.WindowState == FormWindowState.Minimized)
                    _frmSuppliers.WindowState = FormWindowState.Normal;

                _frmSuppliers.Activate();
                _frmSuppliers.BringToFront();
            }
        }

        private void OpenEmployeesModule()
        {
            if (_frmEmployees == null || _frmEmployees.IsDisposed)
            {
                _frmEmployees = new frmEmployees();
                _frmEmployees.FormClosed += (sender, e) => { _frmEmployees = null; };
                _frmEmployees.Show();
            }
            else
            {
                if (_frmEmployees.WindowState == FormWindowState.Minimized)
                    _frmEmployees.WindowState = FormWindowState.Normal;

                _frmEmployees.Activate();
                _frmEmployees.BringToFront();
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            LogoutRequested = true;
            Close();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // frmMain
            // 
            this.ClientSize = new System.Drawing.Size(838, 451);
            this.Name = "frmMain";
            this.ResumeLayout(false);

        }
    }
}
