using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using InventoryManagementSystem.UI;

namespace InventoryManagementSystem.RegressionTests
{
    /// <summary>
    /// Group M (Phase 8A.5): the application shell. One main window, sidebar navigation, every module a page inside the content area.
    /// Runs against the (emptied) test database, so it also proves the empty-state messages.
    /// </summary>
    internal static class Tests_Shell
    {
        private static readonly Color PrimaryBlue = Color.FromArgb(0, 120, 215);
        private static readonly Color DangerRed = Color.FromArgb(200, 35, 51);

        private static readonly string[] Keys =
            { "Dashboard", "Products", "Categories", "StockIn", "StockOut", "Orders", "Customers", "Suppliers", "Employees", "Reports" };

        private static readonly Dictionary<string, Type> PageTypes = new Dictionary<string, Type>
        {
            { "Dashboard", typeof(ucDashboard) }, { "Products", typeof(ucProducts) }, { "Categories", typeof(ucCategories) },
            { "StockIn", typeof(ucStockIn) }, { "StockOut", typeof(ucStockOut) }, { "Orders", typeof(ucOrders) },
            { "Customers", typeof(ucCustomers) }, { "Suppliers", typeof(ucSuppliers) }, { "Employees", typeof(ucEmployees) },
            { "Reports", typeof(ucReports) }
        };

        private static IEnumerable<Control> All(Control root)
        {
            foreach (Control c in root.Controls)
            {
                yield return c;
                foreach (Control d in All(c)) yield return d;
            }
        }

        private static Button FindButton(Control page, string text)
        {
            return All(page).OfType<Button>().FirstOrDefault(b => b.Text == text);
        }

        public static void Run()
        {
            T.Group("M Application shell (UI/UX)");
            T.Wipe();

            frmMain main = Tests_ReportsDashboard.OpenMain("Shell Tester", "Manager");
            Panel content = T.F<Panel>(main, "pnlContent");
            Func<string, Button> nav = key => T.F<Button>(main, "btnNav" + key);

            T.Eq("M01 exactly one top-level window exists after login", 1, Application.OpenForms.Count);
            T.Check("M01 the Dashboard is the first page and it sits in the content area",
                main.CurrentPage is ucDashboard && main.CurrentPage.Parent == content);

            // ---- every module is a page inside the one window -----------------------------------
            UserControl previous = main.CurrentPage;
            foreach (string key in Keys.Skip(1))
            {
                nav(key).PerformClick(); T.Pump();
                UserControl page = main.CurrentPage;

                T.Check("M02 " + key + ": page type is " + PageTypes[key].Name, page != null && page.GetType() == PageTypes[key]);
                T.Check("M02 " + key + ": hosted inside the main content panel (not a separate window)", page.Parent == content && page.FindForm() == main);
                T.Check("M02 " + key + ": content area holds exactly one page", content.Controls.Count == 1 && content.Controls.OfType<UserControl>().Count(c => c.Visible) == 1);
                T.Check("M02 " + key + ": page fills the content area", page.Dock == DockStyle.Fill && page.Size == content.ClientSize, page.Size + " vs " + content.ClientSize);
                T.Check("M03 " + key + ": sidebar highlights exactly this item",
                    Keys.Count(k => nav(k).BackColor == PrimaryBlue) == 1 && nav(key).BackColor == PrimaryBlue);
                T.Check("M04 " + key + ": window title says where the user is", main.Text.EndsWith(key == "StockIn" ? "Stock In" : key == "StockOut" ? "Stock Out" : key));
                T.Check("M05 " + key + ": leaving the previous page released it (no accumulating pages)",
                    previous is ucDashboard ? !previous.IsDisposed : previous.IsDisposed);
                previous = page;
            }

            // ---- no window accumulation ---------------------------------------------------------
            for (int round = 0; round < 3; round++)
                foreach (string key in Keys) { nav(key).PerformClick(); }
            T.Pump();
            T.Eq("M06 after cycling all pages three times there is still exactly one window", 1, Application.OpenForms.Count);
            T.Eq("M06 ... and exactly one page in the content area", 1, content.Controls.Count);

            // ---- re-select behaviour ------------------------------------------------------------
            nav("Products").PerformClick(); T.Pump();
            UserControl products1 = main.CurrentPage;
            nav("Products").PerformClick(); T.Pump();
            T.Check("M07 re-selecting the current page keeps the same page (nothing half-typed is lost)", ReferenceEquals(products1, main.CurrentPage));
            nav("Dashboard").PerformClick(); T.Pump();
            UserControl dash1 = main.CurrentPage;
            nav("Categories").PerformClick(); T.Pump();
            nav("Dashboard").PerformClick(); T.Pump();
            T.Check("M07 the Dashboard instance is reused (and re-queried) when selected again", ReferenceEquals(dash1, main.CurrentPage));
            nav("Products").PerformClick(); T.Pump();
            T.Check("M07 coming back to a module gives a fresh page with fresh data", !ReferenceEquals(products1, main.CurrentPage) && products1.IsDisposed);

            // ---- sidebar hierarchy --------------------------------------------------------------
            FlowLayoutPanel flp = T.F<FlowLayoutPanel>(main, "flpNav");
            List<string> order = flp.Controls.Cast<Control>().Select(c => c.Text).ToList();
            T.Eq("M08 sidebar is grouped: OVERVIEW / MASTER DATA / INVENTORY / OPERATIONS / REPORTING",
                "OVERVIEW|Dashboard|MASTER DATA|Categories|Products|Customers|Suppliers|Employees|INVENTORY|Stock In|Stock Out|OPERATIONS|Orders|REPORTING|Reports", string.Join("|", order));
            T.Check("M08 every item is indented under its group heading", Keys.All(k => nav(k).Padding.Left > T.F<Label>(main, "lblNavInventory").Padding.Left));

            // ---- header -------------------------------------------------------------------------
            T.Eq("M09 header title", "Inventory Management System", T.F<Label>(main, "lblAppTitle").Text);
            T.Eq("M09 header user + role + logout present",
                "Welcome, Shell Tester|Role: Manager|Logout", T.F<Label>(main, "lblUserWelcome").Text + "|" + T.F<Label>(main, "lblUserRole").Text + "|" + T.F<Button>(main, "btnLogout").Text);

            // ---- typography, buttons, grids, empty states, primary actions per page -----------------
            var primary = new Dictionary<string, string>
            {
                { "Dashboard", "Refresh" }, { "Products", "Add" }, { "Categories", "Add" }, { "StockIn", "Record Stock In" }, { "StockOut", "Record Stock Out" },
                { "Orders", "Save Pending Order" }, { "Customers", "Add" }, { "Suppliers", "Add" }, { "Employees", "Add" }, { "Reports", "Apply" }
            };
            var emptyText = new Dictionary<string, string>
            {
                { "Products", "No products found." }, { "Categories", "No categories found." }, { "StockIn", "No stock-in records found." },
                { "StockOut", "No stock-out records found." }, { "Customers", "No customers found." }, { "Suppliers", "No suppliers found." },
                { "Employees", "No employees found." }
            };

            foreach (string key in Keys)
            {
                nav(key).PerformClick(); T.Pump();
                UserControl page = main.CurrentPage;

                T.Check("M10 " + key + ": readable Segoe UI body font (>= 10pt)", page.Font.Name == "Segoe UI" && page.Font.Size >= 10f, page.Font.Name + " " + page.Font.Size);

                Button action = FindButton(page, primary[key]);
                T.Check("M11 " + key + ": obvious primary action '" + primary[key] + "' exists and is styled as primary",
                    action != null && (action.Enabled
                        ? action.BackColor == PrimaryBlue && action.ForeColor == Color.White
                        : action.BackColor == Color.FromArgb(226, 232, 240)));   // transaction buttons start disabled until the form is valid

                var grids = All(page).OfType<DataGridView>().ToList();
                T.Check("M12 " + key + ": grids use the shared style (no grey workspace, no row-header column, one-line headers)",
                    grids.Count > 0 && grids.All(g => g.BackgroundColor == Color.White && !g.RowHeadersVisible &&
                        g.ColumnHeadersDefaultCellStyle.WrapMode == DataGridViewTriState.False && g.DefaultCellStyle.Font.Size >= 10f));

                if (emptyText.ContainsKey(key))
                {
                    DataGridView g = grids.First();
                    Label msg = g.Controls.OfType<Label>().FirstOrDefault(l => l.Text == emptyText[key]);
                    T.Check("M13 " + key + ": empty table shows '" + emptyText[key] + "' instead of a blank area", msg != null && msg.Visible);
                }
            }

            // Orders: two clearly separate areas, history empty message
            nav("Orders").PerformClick(); T.Pump();
            TabControl tabs = T.F<TabControl>(main.CurrentPage, "tabOrders");
            T.Eq("M14 Orders separates entry and history into 'New Order' / 'Order History'", "New Order|Order History", string.Join("|", tabs.TabPages.Cast<TabPage>().Select(t => t.Text)));
            tabs.SelectedIndex = 1; T.Pump();
            DataGridView hist = T.F<DataGridView>(main.CurrentPage, "dgvHistory");
            T.Check("M13 Orders: empty history shows 'No orders found.'", hist.Controls.OfType<Label>().Any(l => l.Text == "No orders found." && l.Visible));
            T.Check("M14 history actions (Confirm / Cancel / View Details) live beside the history grid",
                new[] { "Confirm Order", "Cancel Order", "View Details" }.All(t => FindButton(tabs.TabPages[1], t) != null));
            T.Check("M14 the New Order tab keeps Save Pending Order / Clear", FindButton(tabs.TabPages[0], "Save Pending Order") != null && FindButton(tabs.TabPages[0], "Clear") != null);

            // Button hierarchy on a CRUD page
            nav("Products").PerformClick(); T.Pump();
            Button add = FindButton(main.CurrentPage, "Add"), del = FindButton(main.CurrentPage, "Delete"), clear = FindButton(main.CurrentPage, "Clear");
            T.Check("M15 Delete is disabled (grey, not red) while nothing is selected", !del.Enabled && del.BackColor != DangerRed);
            del.Enabled = true; T.Pump();   // enable it so its enabled style can be verified (UI only, nothing is deleted)
            T.Check("M15 button hierarchy: Add = primary blue, Delete = danger red, Clear = secondary (white)",
                add.BackColor == PrimaryBlue && del.BackColor == DangerRed && clear.BackColor == Color.White && clear.ForeColor != Color.White);

            // ---- resizing --------------------------------------------------------------------------
            foreach (Size size in new[] { new Size(1500, 900), new Size(1100, 640), new Size(1200, 700) })
            {
                main.ClientSize = size; T.Pump();
                foreach (string key in Keys)
                {
                    nav(key).PerformClick(); T.Pump();
                    UserControl page = main.CurrentPage;
                    bool fits = page.Size == content.ClientSize && content.Width > 0 &&
                                nav(key).Right <= flp.ClientSize.Width && T.F<Button>(main, "btnLogout").Right <= main.ClientSize.Width;
                    T.Check("M16 " + key + " @" + size.Width + "x" + size.Height + ": page fills the content area, sidebar and header stay intact", fits);
                }
            }

            main.Close(); T.CloseAllForms();
            T.Eq("M17 everything closes cleanly", 0, Application.OpenForms.Count);
        }
    }
}
