using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using InventoryManagementSystem.BLL;
using InventoryManagementSystem.Entity;
using InventoryManagementSystem.UI;

namespace InventoryManagementSystem.RegressionTests
{
    /// <summary>Groups J (Login / Main navigation) and K (cross-module end-to-end scenario).</summary>
    internal static class Tests_LoginCross
    {
        // =====================================================================================
        public static void RunLoginAndNavigation()
        {
            T.Group("J Login / Main navigation");
            T.Wipe();

            // Test accounts are created with a throw-away random password; no real credential is used or stored anywhere.
            string password = "Pw-" + Guid.NewGuid().ToString("N");
            string hash = PasswordHasher.HashPassword(password);
            const string ins = "INSERT Users(Username,PasswordHash,FullName,Role,IsActive,CreatedAt) VALUES (@p0,@p1,@p2,@p3,@p4,SYSDATETIME())";
            T.Exec(ins, "ZZREG_active", hash, "ZZREG Full Name", "Manager", true);
            T.Exec(ins, "ZZREG_inactive", hash, "ZZREG Inactive", "Staff", false);

            var bll = new UserBLL();
            T.Eq("J00 stored value is a salted PBKDF2 hash, never the password", true, hash != password && !hash.Contains(password) && hash.Split('.').Length == 3);
            T.Check("J00 hashing is salted (same password, different hash)", PasswordHasher.HashPassword(password) != hash);

            T.Eq("J02 empty username -> validation error", LoginStatus.ValidationError, bll.Login("", password).Status);
            T.Eq("J02 empty password -> validation error", LoginStatus.ValidationError, bll.Login("ZZREG_active", "").Status);
            T.Eq("J02 whitespace credentials -> validation error", LoginStatus.ValidationError, bll.Login("  ", "  ").Status);
            T.Eq("J02 null credentials -> validation error", LoginStatus.ValidationError, bll.Login(null, null).Status);

            LoginResult wrongPw = bll.Login("ZZREG_active", password + "x");
            LoginResult noUser = bll.Login("ZZREG_nobody", password);
            T.Eq("J03 wrong password -> InvalidCredentials", LoginStatus.InvalidCredentials, wrongPw.Status);
            T.Eq("J03 unknown user -> InvalidCredentials", LoginStatus.InvalidCredentials, noUser.Status);
            T.Eq("J03 identical generic message for both (no user enumeration)", wrongPw.Message, noUser.Message);
            T.Check("J03 message does not reveal which part was wrong", !wrongPw.Message.ToLower().Contains("password is") && !noUser.Message.ToLower().Contains("no such"));
            T.Eq("J03 password is case-sensitive", LoginStatus.InvalidCredentials, bll.Login("ZZREG_active", password.ToUpper()).Status);
            T.Check("J03 SQL-injection style username is just a failed login", bll.Login("' OR '1'='1", "x").Status == LoginStatus.InvalidCredentials);

            LoginResult inactive = bll.Login("ZZREG_inactive", password);
            T.Eq("J04 inactive user with the right password is refused", LoginStatus.InactiveUser, inactive.Status);
            T.Check("J04 no user object is returned for an inactive account", inactive.User == null);
            T.Eq("J04 inactive user with a wrong password still gets the generic message", LoginStatus.InvalidCredentials, bll.Login("ZZREG_inactive", "nope").Status);

            LoginResult ok = bll.Login("  ZZREG_active  ", password);
            T.Eq("J05 valid login succeeds (username is trimmed)", LoginStatus.Success, ok.Status);
            T.Eq("J05 returned user FullName|Role", "ZZREG Full Name|Manager", ok.User == null ? "null" : ok.User.FullName + "|" + ok.User.Role);

            // ---- login form (invalid paths would raise a modal MessageBox, so they are covered at BLL level above)
            var login = new frmLogin();
            T.Check("J01 login form constructs", login != null);
            T.Check("J01 password box masks input", T.F<TextBox>(login, "txtPassword").UseSystemPasswordChar || T.F<TextBox>(login, "txtPassword").PasswordChar != '\0');
            login.Show(); T.Pump(); // PerformClick only works on a shown form
            T.F<TextBox>(login, "txtUsername").Text = "ZZREG_active";
            T.F<TextBox>(login, "txtPassword").Text = "typed";
            T.F<Button>(login, "btnClear").PerformClick(); T.Pump();
            T.Check("J01 Clear button empties both fields", T.F<TextBox>(login, "txtUsername").Text == "" && T.F<TextBox>(login, "txtPassword").Text == "");
            T.F<TextBox>(login, "txtUsername").Text = "ZZREG_active";
            T.F<TextBox>(login, "txtPassword").Text = password;
            T.F<Button>(login, "btnLogin").PerformClick(); T.Pump();
            T.Check("J05 valid login through the form authenticates", login.AuthenticatedUser != null && login.AuthenticatedUser.Username == "ZZREG_active");
            T.Check("J05 login form reports DialogResult.OK", login.DialogResult == DialogResult.OK);

            // ---- main form ---------------------------------------------------------------------
            var main = new frmMain(login.AuthenticatedUser) { ShowInTaskbar = false };
            main.Show(); T.Pump();
            T.Eq("J06 main form opens after login", true, main.Visible);
            T.Eq("J07 header shows the FullName", "Welcome, ZZREG Full Name", T.F<Label>(main, "lblUserWelcome").Text);
            T.Eq("J07 header shows the Role", "Role: Manager", T.F<Label>(main, "lblUserRole").Text);
            T.Check("J06 Dashboard is the initial module", main.CurrentPage is ucDashboard && main.CurrentModule == "Dashboard");

            // Phase 8A.5: modules are pages shown INSIDE the main window; navigation never opens another window.
            var nav = new[]
            {
                Tuple.Create("btnNavCategories", typeof(ucCategories)), Tuple.Create("btnNavProducts", typeof(ucProducts)),
                Tuple.Create("btnNavCustomers", typeof(ucCustomers)), Tuple.Create("btnNavSuppliers", typeof(ucSuppliers)),
                Tuple.Create("btnNavEmployees", typeof(ucEmployees)), Tuple.Create("btnNavStockIn", typeof(ucStockIn)),
                Tuple.Create("btnNavStockOut", typeof(ucStockOut)), Tuple.Create("btnNavOrders", typeof(ucOrders)),
                Tuple.Create("btnNavReports", typeof(ucReports))
            };
            foreach (var n in nav)
            {
                T.F<Button>(main, n.Item1).PerformClick(); T.Pump();
                T.Check("J09 navigation shows " + n.Item2.Name + " inside the main window",
                    main.CurrentPage != null && main.CurrentPage.GetType() == n.Item2 && main.CurrentPage.Visible && !main.CurrentPage.IsDisposed);
            }
            T.F<Button>(main, "btnNavDashboard").PerformClick(); T.Pump();
            T.Check("J09 navigation returns to the Dashboard", main.CurrentPage is ucDashboard);
            foreach (var n in nav) T.F<Button>(main, n.Item1).PerformClick();
            T.Pump();
            T.Check("J09 no extra top-level windows were created by navigating (only the main form exists)",
                Application.OpenForms.Count == 1 && Application.OpenForms[0] == main);
            T.CloseAllForms();
            T.Check("J09 everything closes cleanly", Application.OpenForms.Count == 0);

            // ---- logout ------------------------------------------------------------------------
            var main2 = new frmMain(new UserEntity { FullName = "x", Role = "y" }) { ShowInTaskbar = false };
            main2.Show(); T.Pump();
            main2.Close(); T.Pump();
            T.Check("J08 closing the window normally does NOT request logout (app exits)", !main2.LogoutRequested);

            var main3 = new frmMain(login.AuthenticatedUser) { ShowInTaskbar = false };
            main3.Show(); T.Pump();
            T.F<Button>(main3, "btnLogout").PerformClick(); T.Pump();
            T.Check("J08 Logout sets LogoutRequested (Program then re-shows the login form) and closes the main form", main3.LogoutRequested && main3.IsDisposed);
            T.CloseAllForms();
        }

        // =====================================================================================
        public static void RunCrossModule()
        {
            T.Group("K Cross-module scenario");
            T.Wipe();

            // Pre-existing "user" data that the scenario's cleanup must not touch.
            int preCat = T.NewCategory("ZZREG_Existing");
            T.NewProduct("ZZREG_ExistingProd", preCat, 5, 12m, 5);
            T.NewCustomer("ZZREG_ExistingCust");
            string baseline = T.Snapshot();
            var identities = T.Tables.ToDictionary(t => t, t => T.IdentityLast(t));

            // 1-2 category + product (through the BLLs, as the UI does)
            T.Check("K01 create category", new CategoryBLL().Add(new CategoryEntity { CategoryName = "ZZREG_XCat" }).Success);
            int cat = T.Id("SELECT CategoryID FROM Categories WHERE CategoryName='ZZREG_XCat'");
            T.Check("K02 create product (stock 0, price 20, reorder 5)",
                new ProductBLL().Add(new ProductEntity { ProductName = "ZZREG_XProd", CategoryID = cat, UnitPrice = 20m, QtyInStock = 0, ReorderLevel = 5 }).Success);
            int prod = T.Id("SELECT ProductID FROM Products WHERE ProductName='ZZREG_XProd'");
            T.Check("K03 create supplier", new SupplierBLL().Add(new SupplierEntity { SupplierName = "ZZREG_XSup" }).Success);
            int sup = T.Id("SELECT SupplierID FROM Suppliers WHERE SupplierName='ZZREG_XSup'");

            // 4-5 stock in
            T.Check("K04 record Stock In (10 @ 3.00)", new StockInBLL().Add(new StockInEntity { ProductID = prod, SupplierID = sup, Quantity = 10, UnitCost = 3m }).Success);
            T.Eq("K05 stock rose to 10", 10, T.Stock(prod));

            // 6-9 customer, employee, stock out
            T.Check("K06 create customer", new CustomerBLL().Add(new CustomerEntity { CustomerName = "ZZREG_XCust" }).Success);
            int cust = T.Id("SELECT CustomerID FROM Customers WHERE CustomerName='ZZREG_XCust'");
            T.Check("K07 create employee", new EmployeeBLL().Add(new EmployeeEntity { EmployeeName = "ZZREG_XEmp" }).Success);
            int emp = T.Id("SELECT EmployeeID FROM Employees WHERE EmployeeName='ZZREG_XEmp'");
            T.Check("K08 record Stock Out (2 @ 25.00)", new StockOutBLL().Add(new StockOutEntity { ProductID = prod, CustomerID = cust, Quantity = 2, UnitPrice = 25m }).Success);
            T.Eq("K09 stock fell to 8", 8, T.Stock(prod));

            // 10-14 order
            var ob = new OrderBLL();
            OrderResult created = ob.CreateOrder(new OrderEntity { CustomerID = cust, EmployeeID = emp },
                new List<OrderDetailEntity> { new OrderDetailEntity { ProductID = prod, Quantity = 3, UnitPrice = 20m } });
            T.Check("K10/11 order with one detail created", created.Success);
            T.Eq("K10 order is Pending and stock is still 8", "Pending|8", T.Str("SELECT Status FROM Orders WHERE OrderID=@p0", created.OrderID) + "|" + T.Stock(prod));
            long stockOutRows = T.Count("StockOut");
            T.Check("K12 confirm the order", ob.ConfirmOrder(created.OrderID).Success);
            T.Eq("K13 stock deducted exactly once (8 - 3 = 5)", 5, T.Stock(prod));
            T.Eq("K14 confirmation created no StockOut row", stockOutRows, T.Count("StockOut"));

            // 15-16 reports
            var rb = new ReportBLL();
            StockInReportResult rin = rb.GetStockInReport(prod);
            T.Eq("K16 Stock In report shows the receipt", "1|10|30.00", rin.Rows.Count + "|" + rin.TotalQuantity + "|" + rin.TotalCost.ToString("0.00"));
            StockOutReportResult rout = rb.GetStockOutReport(prod);
            T.Eq("K16 Stock Out report shows ONLY the manual issue (order confirmation is not a StockOut)", "1|2|50.00", rout.Rows.Count + "|" + rout.TotalQuantity + "|" + rout.TotalValue.ToString("0.00"));
            OrderReportResult rord = rb.GetOrderReport("Confirmed", cust);
            T.Eq("K16 Order report shows the confirmed order", "1|60.00", rord.OrderCount + "|" + rord.TotalAmount.ToString("0.00"));
            InventoryReportResult rinv = rb.GetInventoryReport(null, null, "ZZREG_XProd");
            T.Eq("K16 Inventory report shows stock 5 and Low Stock (5 <= reorder 5)", "5|Reorder / Low Stock", rinv.Rows.Single().QtyInStock + "|" + rinv.Rows.Single().StockStatus);

            var form = T.HostPage<ucReports>();
            T.F<RadioButton>(form, "rdoOrders").Checked = true; T.Pump();
            T.Check("K15 Reports form opens and lists the new order", form.Visible && T.F<DataGridView>(form, "dgvReport").Rows.Count == 1);
            T.ClosePage(form);

            // 17-18 dashboard
            frmMain main = Tests_ReportsDashboard.OpenMain("K Tester", "Admin");
            ucDashboard dash = T.F<ucDashboard>(main, "_dashboard");
            Func<string, string> card = k => T.F<Dictionary<string, Label>>(dash, "_cardValues")[k].Text;
            T.Eq("K18 Dashboard KPIs reflect the transactions (products|categories|units|low|in|out|confirmed|pending)",
                "2|2|10|2|10|2|1|0",
                string.Join("|", new[] { "Products", "Categories", "Units", "LowStock", "StockIn", "StockOut", "Confirmed", "Pending" }.Select(card)));
            var low = T.F<DataGridView>(dash, "dgvLowStock").Rows.Cast<DataGridViewRow>().Select(r => ((LowStockItem)r.DataBoundItem).ProductName).ToList();
            T.Check("K18 Dashboard low-stock list includes the scenario product", low.Contains("ZZREG_XProd"));
            var recent = T.F<DataGridView>(dash, "dgvRecent").Rows.Cast<DataGridViewRow>().Select(r => (RecentActivityItem)r.DataBoundItem).ToList();
            T.Check("K17 Dashboard recent activity shows Stock In, Stock Out and the confirmed Order",
                recent.Any(r => r.ActivityType == "Stock In") && recent.Any(r => r.ActivityType == "Stock Out") &&
                recent.Any(r => r.ActivityType == "Order" && r.Description.Contains("ZZREG_XCust") && r.Description.Contains("Confirmed") && r.QtyOrAmount == "60.00"));
            main.Close(); T.CloseAllForms();

            // 19-20 order details keep the historical price even after the product price changes
            ProductEntity pe = new ProductBLL().GetAll().Single(p => p.ProductID == prod);
            pe.UnitPrice = 99m;
            T.Check("K19 product price changed to 99.00", new ProductBLL().Update(pe).Success);
            OrderEntity oe = ob.GetAll().Single(o => o.OrderID == created.OrderID);
            var od = new frmOrderDetails(oe) { ShowInTaskbar = false };
            od.Show(); T.Pump();
            T.Eq("K20 Order Details header total", "60.00", T.F<TextBox>(od, "txtTotalAmount").Text);
            T.Eq("K20 Order Details status", "Confirmed", T.F<TextBox>(od, "txtStatus").Text);
            DataGridView items = T.F<DataGridView>(od, "dgvItems");
            T.Eq("K20 one line item", 1, items.Rows.Count);
            T.Eq("K20 historical unit price 20.00 (not the new 99.00)", "20.00", items.Rows[0].Cells[2].FormattedValue.ToString());
            T.Eq("K20 historical line total 60.00", "60.00", items.Rows[0].Cells[3].FormattedValue.ToString());
            T.Check("K20 Order Details is read-only", items.ReadOnly && T.F<TextBox>(od, "txtTotalAmount").ReadOnly && T.F<TextBox>(od, "txtStatus").ReadOnly);
            od.Close();

            // 21-22 targeted cleanup (tracked ids only), identity counters restored, baseline verified
            T.Exec(@"DELETE FROM OrderDetails WHERE OrderID=@p0; DELETE FROM Orders WHERE OrderID=@p0;
                     DELETE FROM StockOut WHERE ProductID=@p1; DELETE FROM StockIn WHERE ProductID=@p1;
                     DELETE FROM Products WHERE ProductID=@p1; DELETE FROM Categories WHERE CategoryID=@p2;
                     DELETE FROM Suppliers WHERE SupplierID=@p3; DELETE FROM Customers WHERE CustomerID=@p4;
                     DELETE FROM Employees WHERE EmployeeID=@p5;", created.OrderID, prod, cat, sup, cust, emp);
            foreach (var kv in identities) T.RestoreIdentity(kv.Key, kv.Value);
            T.Eq("K21/K22 after cleanup the database is back at the exact baseline (rows, checksums, identity counters)", baseline, T.Snapshot());
            T.Eq("K22 the pre-existing records survived the cleanup", 1L,
                T.Scalar("SELECT COUNT(*) FROM Products WHERE ProductName='ZZREG_ExistingProd' AND QtyInStock=5 AND UnitPrice=12"));
        }
    }
}
