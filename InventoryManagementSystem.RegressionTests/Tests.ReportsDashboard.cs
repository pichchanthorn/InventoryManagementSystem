using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using InventoryManagementSystem.BLL;
using InventoryManagementSystem.Entity;
using InventoryManagementSystem.UI;

namespace InventoryManagementSystem.RegressionTests
{
    /// <summary>Groups H (Reports) and I (Dashboard).</summary>
    internal static class Tests_ReportsDashboard
    {
        private const string NoRecords = "No records found for the selected filters.";
        private static readonly string[] CardKeys = { "Products", "Categories", "Units", "LowStock", "StockIn", "StockOut", "Confirmed", "Pending" };

        // ---- deterministic fixture shared by groups H and I -----------------------------------
        private sealed class Fx { public int Cat, Out, Low, Edge, Ok, Sup, C1, C2, Emp; }

        private static Fx Seed()
        {
            T.Wipe();
            var f = new Fx();
            f.Cat = T.NewCategory("ZZREG_Cat");
            f.Out = T.NewProduct("ZZREG_Out", f.Cat, 0, 10m, 5);
            f.Low = T.NewProduct("ZZREG_Low", f.Cat, 2, 10m, 5);
            f.Edge = T.NewProduct("ZZREG_Edge", f.Cat, 5, 15m, 5);
            f.Ok = T.NewProduct("ZZREG_Ok", f.Cat, 50, 20m, 5);
            f.Sup = T.NewSupplier("ZZREG_Sup");
            f.C1 = T.NewCustomer("ZZREG_Cust1");
            f.C2 = T.NewCustomer("ZZREG_Cust2");
            f.Emp = T.NewEmployee("ZZREG_Emp");

            const string si = "INSERT StockIn(ProductID,SupplierID,Quantity,UnitCost,TotalCost,DateIn,Notes) VALUES (@p0,@p1,@p2,@p3,@p4,@p5,@p6)";
            T.Exec(si, f.Ok, f.Sup, 10, 2.50m, 25m, "2026-01-10 08:00:00", "ZZREG note alpha");
            T.Exec(si, f.Ok, f.Sup, 20, 3.00m, 60m, "2026-01-15 23:59:59.5", null);
            T.Exec(si, f.Low, null, 5, 4.00m, 20m, "2026-02-01 00:00:00", null);

            const string so = "INSERT StockOut(ProductID,CustomerID,Quantity,UnitPrice,TotalPrice,DateOut,Notes) VALUES (@p0,@p1,@p2,@p3,@p4,@p5,@p6)";
            T.Exec(so, f.Ok, f.C1, 2, 20m, 40m, "2026-01-10 08:00:00", "ZZREG note beta");
            T.Exec(so, f.Ok, f.C1, 3, 20m, 60m, "2026-01-15 23:59:59.5", null);
            T.Exec(so, f.Low, f.C2, 1, 10m, 10m, "2026-02-01 00:00:00", null);
            T.Exec(so, f.Low, null, 4, 10m, 40m, "2026-02-02 10:00:00", null);

            const string od = "INSERT Orders(CustomerID,EmployeeID,OrderDate,TotalAmount,Status) OUTPUT INSERTED.OrderID VALUES (@p0,@p1,@p2,@p3,@p4)";
            int o1 = T.Id(od, f.C1, f.Emp, "2026-01-10 08:00:00", 100.50m, "Pending");
            T.Id(od, f.C1, f.Emp, "2026-01-15 23:59:59.5", 200m, "Confirmed");
            T.Id(od, f.C2, f.Emp, "2026-02-01 00:00:00", 50.25m, "Cancelled");
            T.Id(od, null, null, "2026-02-02 10:00:00", 75m, "Confirmed");
            // two detail lines on one order: a join-based total would double count it
            T.Exec("INSERT OrderDetails(OrderID,ProductID,Quantity,UnitPrice,Total) VALUES (@p0,@p1,2,20,40),(@p0,@p2,3,20.17,60.50)", o1, f.Ok, f.Low);
            return f;
        }

        private static void SelectByText(ComboBox c, string text)
        {
            for (int i = 0; i < c.Items.Count; i++) { c.SelectedIndex = i; if (c.Text == text) return; }
            throw new InvalidOperationException("Combo item not found: " + text);
        }

        // =====================================================================================
        public static void RunEmptyStates()
        {
            T.Wipe();
            T.Group("H Reports");
            var rb = new ReportBLL();
            T.Eq("H-empty inventory report empty, summary zeros",
                "0|0|0", rb.GetInventoryReport().Rows.Count + "|" + rb.GetInventoryReport().TotalUnitsInStock + "|" + rb.GetInventoryReport().LowStockCount);
            T.Eq("H-empty stock in report", "0|0|0", Summ(rb.GetStockInReport()));
            T.Eq("H-empty stock out report", "0|0|0", Summ(rb.GetStockOutReport()));
            T.Eq("H-empty order report", "0|0", rb.GetOrderReport().OrderCount + "|" + rb.GetOrderReport().TotalAmount);

            var form = T.HostPage<ucReports>();
            foreach (string rdo in new[] { "rdoInventory", "rdoStockIn", "rdoStockOut", "rdoOrders" })
            {
                T.F<RadioButton>(form, rdo).Checked = true; T.Pump();
                T.Check("H-empty " + rdo + " shows the friendly empty message and no fake totals",
                    T.F<DataGridView>(form, "dgvReport").Rows.Count == 0 && T.F<Label>(form, "lblMessage").Text == NoRecords && T.F<Label>(form, "lblSummary").Text == "");
            }
            T.ClosePage(form);

            T.Group("I Dashboard");
            var main = OpenMain("Empty Tester", "Admin");
            var dash = T.F<ucDashboard>(main, "_dashboard");
            T.Check("I07 empty DB: dashboard opens", main.Visible && dash.Visible);
            T.Check("I07 empty DB: all eight KPIs show 0 (SUM over no rows is 0, not blank)", CardKeys.All(k => Card(dash, k) == "0"),
                string.Join(",", CardKeys.Select(k => Card(dash, k))));
            T.Check("I07 empty low-stock message shown, grid hidden", T.F<Label>(dash, "lblLowStockEmpty").Visible && !T.F<DataGridView>(dash, "dgvLowStock").Visible);
            T.Check("I07 empty recent-activity message shown, grid hidden", T.F<Label>(dash, "lblRecentEmpty").Visible && !T.F<DataGridView>(dash, "dgvRecent").Visible);
            T.Check("I07 last-refreshed timestamp still shown", T.F<Label>(dash, "lblLastRefreshed").Text.StartsWith("Last refreshed: "));
            main.Close(); T.CloseAllForms();
        }

        private static string Summ(StockInReportResult r) { return r.Rows.Count + "|" + r.TotalQuantity + "|" + r.TotalCost; }
        private static string Summ(StockOutReportResult r) { return r.Rows.Count + "|" + r.TotalQuantity + "|" + r.TotalValue; }

        // =====================================================================================
        public static void RunReports()
        {
            T.Group("H Reports");
            Fx f = Seed();
            var rb = new ReportBLL();
            string snap = T.Snapshot();

            // ---- inventory ---------------------------------------------------------------------
            InventoryReportResult inv = rb.GetInventoryReport();
            T.Eq("H01 inventory rows", 4, inv.Rows.Count);
            T.Eq("H01 inventory summary (products|units|low)", "4|57|3", inv.TotalProducts + "|" + inv.TotalUnitsInStock + "|" + inv.LowStockCount);
            T.Eq("H01 status derivation: stock 0 / 2 / 5(=reorder) / 50", "Reorder / Low Stock|Reorder / Low Stock|Reorder / Low Stock|In Stock",
                string.Join("|", new[] { "ZZREG_Out", "ZZREG_Low", "ZZREG_Edge", "ZZREG_Ok" }.Select(n => inv.Rows.Single(r => r.ProductName == n).StockStatus)));
            T.Eq("H01 category shown", "ZZREG_Cat", inv.Rows[0].CategoryName);
            T.Eq("H02 category filter (matching)", 4, rb.GetInventoryReport(f.Cat).Rows.Count);
            T.Eq("H02 category filter (other id)", 0, rb.GetInventoryReport(f.Cat + 100).Rows.Count);
            InventoryReportResult low = rb.GetInventoryReport(null, ReportBLL.InventoryStatusLowStock);
            T.Eq("H02 low-stock filter rows/summary", "3|3", low.Rows.Count + "|" + low.LowStockCount);
            InventoryReportResult ins = rb.GetInventoryReport(null, ReportBLL.InventoryStatusInStock);
            T.Eq("H02 in-stock filter rows/summary", "1|50|0", ins.Rows.Count + "|" + ins.TotalUnitsInStock + "|" + ins.LowStockCount);
            T.Eq("H03 search by product name", 1, rb.GetInventoryReport(null, null, "ZZREG_Lo").Rows.Count);
            T.Eq("H03 search by category name", 4, rb.GetInventoryReport(null, null, "ZZREG_Cat").Rows.Count);
            T.Eq("H03 search is case-insensitive and trimmed", 1, rb.GetInventoryReport(null, null, "  zzreg_edge ").Rows.Count);
            T.Eq("H03 filters combine (category + low + search)", 1, rb.GetInventoryReport(f.Cat, ReportBLL.InventoryStatusLowStock, "Edge").Rows.Count);
            T.Eq("H03 no match -> empty list, not an error", 0, rb.GetInventoryReport(null, null, "no-such-thing").Rows.Count);
            T.Eq("H08 literal '%' matches nothing (wildcards escaped - current behaviour)", 0, rb.GetInventoryReport(null, null, "%").Rows.Count);
            T.Eq("H08 literal '_' matches nothing (wildcards escaped - current behaviour)", 0, rb.GetInventoryReport(null, null, "ZZREG_Lo_").Rows.Count);
            T.Eq("H08 literal '[' does not break the query", 0, rb.GetInventoryReport(null, null, "[abc").Rows.Count);
            T.Eq("H07 SQL-injection style search is inert", 0, rb.GetInventoryReport(null, null, "'; DROP TABLE Products;--").Rows.Count);
            T.Eq("H07 ... and Products still exists with its rows", 4L, T.Count("Products"));
            T.Eq("H07 ' OR '1'='1 matches nothing", 0, rb.GetStockInReport(null, null, null, null, "' OR '1'='1").Rows.Count);

            // ---- stock in ----------------------------------------------------------------------
            StockInReportResult si = rb.GetStockInReport();
            T.Eq("H04 stock in rows|qty|cost", "3|35|105.00", si.Rows.Count + "|" + si.TotalQuantity + "|" + si.TotalCost.ToString("0.00"));
            T.Eq("H05 product filter", "2|30|85.00", Summ2(rb.GetStockInReport(f.Ok)));
            T.Eq("H05 supplier filter", 2, rb.GetStockInReport(null, f.Sup).Rows.Count);
            T.Eq("H06 date range 10th-15th includes the 15th 23:59:59.5 row", 2, rb.GetStockInReport(null, null, D("2026-01-10"), D("2026-01-15")).Rows.Count);
            T.Eq("H06 date range 10th-14th excludes it", 1, rb.GetStockInReport(null, null, D("2026-01-10"), D("2026-01-14")).Rows.Count);
            T.Eq("H06 empty gap range", 0, rb.GetStockInReport(null, null, D("2026-01-16"), D("2026-01-31")).Rows.Count);
            T.Eq("H06 single day incl. 00:00:00 row", 1, rb.GetStockInReport(null, null, D("2026-02-01"), D("2026-02-01")).Rows.Count);
            T.Eq("H06 open-ended From", 1, rb.GetStockInReport(null, null, D("2026-01-16"), null).Rows.Count);
            T.Eq("H06 open-ended To", 2, rb.GetStockInReport(null, null, null, D("2026-01-15")).Rows.Count);
            T.Eq("H03 search on notes", 1, rb.GetStockInReport(null, null, null, null, "alpha").Rows.Count);
            T.Eq("H03 search on supplier name", 2, rb.GetStockInReport(null, null, null, null, "ZZREG_Sup").Rows.Count);
            T.Eq("H03 search on product name", 2, rb.GetStockInReport(null, null, null, null, "ZZREG_Ok").Rows.Count);
            T.Check("H09 NULL supplier row is returned with a null name (no crash)", si.Rows.Any(r => r.SupplierName == null));
            AssertThrows<ArgumentException>("H06 From later than To is rejected by the BLL", () => rb.GetStockInReport(null, null, D("2026-02-01"), D("2026-01-01")));

            // ---- stock out (reads StockOut only) -----------------------------------------------
            StockOutReportResult so = rb.GetStockOutReport();
            T.Eq("H04 stock out rows|qty|value", "4|10|150.00", so.Rows.Count + "|" + so.TotalQuantity + "|" + so.TotalValue.ToString("0.00"));
            T.Eq("H04 StockOut report is NOT derived from Orders (2 confirmed orders exist, 4 StockOut rows reported)", 4, so.Rows.Count);
            T.Eq("H05 product filter", 2, rb.GetStockOutReport(f.Ok).Rows.Count);
            T.Eq("H05 customer filter", "2|100.00", Summ3(rb.GetStockOutReport(null, f.C1)));
            T.Eq("H06 date range 10th-15th", 2, rb.GetStockOutReport(null, null, D("2026-01-10"), D("2026-01-15")).Rows.Count);
            T.Eq("H06 range 2nd Feb only", 1, rb.GetStockOutReport(null, null, D("2026-02-02"), D("2026-02-02")).Rows.Count);
            T.Eq("H03 search on notes", 1, rb.GetStockOutReport(null, null, null, null, "beta").Rows.Count);
            T.Eq("H03 search on customer name", 1, rb.GetStockOutReport(null, null, null, null, "Cust2").Rows.Count);
            T.Check("H09 NULL customer row is returned with a null name (no crash)", so.Rows.Any(r => r.CustomerName == null));

            // ---- orders (no OrderDetails double counting) --------------------------------------
            OrderReportResult or = rb.GetOrderReport();
            T.Eq("H04 orders count|total|P|C|X", "4|425.75|1|2|1",
                or.OrderCount + "|" + or.TotalAmount.ToString("0.00") + "|" + or.PendingCount + "|" + or.ConfirmedCount + "|" + or.CancelledCount);
            T.Eq("H04 order total equals SUM(Orders.TotalAmount)", T.Dec("SELECT SUM(TotalAmount) FROM Orders"), or.TotalAmount);
            T.Eq("H04 fixture check: joining Orders to OrderDetails WOULD double count order 1 (2 x 100.50)", 201.00m,
                T.Dec("SELECT SUM(o.TotalAmount) FROM Orders o INNER JOIN OrderDetails d ON d.OrderID=o.OrderID"));
            T.Eq("H05 status Pending", 1, rb.GetOrderReport("Pending").Rows.Count);
            T.Eq("H05 status Confirmed", "2|275.00", rb.GetOrderReport("Confirmed").OrderCount + "|" + rb.GetOrderReport("Confirmed").TotalAmount.ToString("0.00"));
            T.Eq("H05 status Cancelled", 1, rb.GetOrderReport("Cancelled").Rows.Count);
            T.Eq("H05 customer filter", 2, rb.GetOrderReport(null, f.C1).Rows.Count);
            T.Eq("H06 date range 10th-15th", 2, rb.GetOrderReport(null, null, D("2026-01-10"), D("2026-01-15")).Rows.Count);
            T.Eq("H03 search on employee name", 3, rb.GetOrderReport(null, null, null, null, "ZZREG_Emp").Rows.Count);
            int confirmedNoEmp = T.Id("SELECT OrderID FROM Orders WHERE EmployeeID IS NULL");
            T.Eq("H03 search on order id", 1, rb.GetOrderReport(null, null, null, null, confirmedNoEmp.ToString()).Rows.Count);
            T.Check("H09 order with NULL employee is returned (no crash)", or.Rows.Any(r => r.EmployeeName == null));

            T.Eq("H10 running every report left the database unchanged", snap, T.Snapshot());

            // ---- UI smoke: real form driving the same BLL --------------------------------------
            var form = T.HostPage<ucReports>();
            Func<int> rows = () => T.F<DataGridView>(form, "dgvReport").Rows.Count;
            Func<string> sum = () => T.F<Label>(form, "lblSummary").Text;
            Action<string> click = n => { T.F<Button>(form, n).PerformClick(); T.Pump(); };
            Action<string> report = n => { T.F<RadioButton>(form, n).Checked = true; T.Pump(); };

            T.Check("H11 Reports form opens on the Inventory report", form.Visible && rows() == 4);
            T.Eq("H11 inventory summary text", "Total products: 4     Total units in stock: 57     Low-stock products: 3", sum());
            T.F<TextBox>(form, "txtSearch").Text = "ZZREG_Lo"; click("btnApply");
            T.Eq("H11 Apply with a search filters the grid", 1, rows());
            click("btnRefresh"); T.Eq("H11 Refresh re-runs and keeps the filters", 1, rows());
            click("btnClearFilters"); T.Eq("H11 Clear Filters restores the full report", 4, rows());
            SelectByText(T.F<ComboBox>(form, "cmbStatus"), ReportBLL.InventoryStatusLowStock); click("btnApply");
            T.Eq("H11 low-stock status filter through the UI", 3, rows());
            click("btnClearFilters");

            report("rdoStockIn");
            T.Check("H11 Stock In report loads", rows() == 3 && sum() == "Stock In transactions: 3     Total quantity received: 35     Total cost: $105.00", sum());
            T.F<CheckBox>(form, "chkDateFilter").Checked = true;
            T.F<DateTimePicker>(form, "dtpFrom").Value = D("2026-01-10"); T.F<DateTimePicker>(form, "dtpTo").Value = D("2026-01-15"); click("btnApply");
            T.Eq("H11 date range through the UI is inclusive of the To day", 2, rows());
            T.F<DateTimePicker>(form, "dtpFrom").Value = D("2026-01-16"); T.F<DateTimePicker>(form, "dtpTo").Value = D("2026-01-31"); click("btnApply");
            T.Check("H11 empty result shows the friendly message", rows() == 0 && T.F<Label>(form, "lblMessage").Text == NoRecords && sum() == "");
            click("btnClearFilters"); T.Eq("H11 Clear Filters restores Stock In", 3, rows());
            report("rdoStockOut");
            T.Check("H11 Stock Out report loads", rows() == 4 && sum() == "Stock Out transactions: 4     Total quantity issued: 10     Total value: $150.00", sum());
            report("rdoOrders");
            T.Check("H11 Order report loads", rows() == 4 && sum() == "Orders: 4     Total order amount: $425.75     Pending: 1     Confirmed: 2     Cancelled: 1", sum());
            SelectByText(T.F<ComboBox>(form, "cmbStatus"), "Confirmed"); click("btnApply");
            T.Eq("H11 order status filter through the UI", 2, rows());
            var grid = T.F<DataGridView>(form, "dgvReport");
            T.Check("H11 report grid is read-only (no add/delete/edit)", grid.ReadOnly && !grid.AllowUserToAddRows && !grid.AllowUserToDeleteRows);
            T.ClosePage(form);
            T.Eq("H10 the Reports UI left the database unchanged", snap, T.Snapshot());
        }

        private static string Summ2(StockInReportResult r) { return r.Rows.Count + "|" + r.TotalQuantity + "|" + r.TotalCost.ToString("0.00"); }
        private static string Summ3(StockOutReportResult r) { return r.Rows.Count + "|" + r.TotalValue.ToString("0.00"); }
        private static DateTime D(string s) { return DateTime.Parse(s); }

        private static void AssertThrows<TEx>(string name, Action a) where TEx : Exception
        {
            try { a(); T.Check(name, false, "no exception"); }
            catch (TEx) { T.Check(name, true); }
            catch (Exception ex) { T.Check(name, false, "wrong exception " + ex.GetType().Name); }
        }

        // =====================================================================================
        internal static frmMain OpenMain(string fullName, string role)
        {
            var main = new frmMain(new UserEntity { FullName = fullName, Role = role }) { ShowInTaskbar = false };
            main.Show(); T.Pump();
            return main;
        }

        private static string Card(ucDashboard d, string key) { return T.F<Dictionary<string, Label>>(d, "_cardValues")[key].Text; }

        public static void RunDashboard()
        {
            T.Group("I Dashboard");
            Fx f = Seed();
            frmMain main = OpenMain("Dash Tester", "Admin");
            ucDashboard dash = T.F<ucDashboard>(main, "_dashboard");
            T.Check("I01 dashboard opens with the main form", main.Visible && dash.Visible);

            var expected = new Dictionary<string, string>
            {
                { "Products", "4" }, { "Categories", "1" }, { "Units", "57" }, { "LowStock", "3" },
                { "StockIn", "35" }, { "StockOut", "10" }, { "Confirmed", "2" }, { "Pending", "1" }
            };
            foreach (var kv in expected)
                T.Eq("I02 KPI " + kv.Key, kv.Value, Card(dash, kv.Key));
            // independent oracle for the same numbers
            T.Eq("I02 Total Products vs SQL", T.Scalar("SELECT COUNT(*) FROM Products").ToString("N0"), Card(dash, "Products"));
            T.Eq("I02 Total Units vs SQL", T.Scalar("SELECT SUM(QtyInStock) FROM Products").ToString("N0"), Card(dash, "Units"));
            T.Eq("I02 Low Stock vs SQL", T.Scalar("SELECT COUNT(*) FROM Products WHERE QtyInStock<=ReorderLevel").ToString("N0"), Card(dash, "LowStock"));

            var low = T.F<DataGridView>(dash, "dgvLowStock");
            List<LowStockItem> lowItems = low.Rows.Cast<DataGridViewRow>().Select(r => (LowStockItem)r.DataBoundItem).ToList();
            T.Eq("I03 low-stock list = products with stock <= reorder level", "ZZREG_Edge,ZZREG_Low,ZZREG_Out",
                string.Join(",", lowItems.Select(i => i.ProductName).OrderBy(x => x)));
            T.Check("I03 list is ordered lowest stock first", lowItems.Select(i => i.QtyInStock).SequenceEqual(new[] { 0, 2, 5 }));
            T.Check("I03 stock equal to reorder level is included", lowItems.Any(i => i.ProductName == "ZZREG_Edge"));
            T.Check("I03 in-stock product excluded", lowItems.All(i => i.ProductName != "ZZREG_Ok"));
            T.Eq("I04 zero stock shows 'Out of Stock'", "Out of Stock", lowItems.Single(i => i.ProductName == "ZZREG_Out").Status);
            T.Eq("I04 other low items show 'Low Stock'", "Low Stock|Low Stock", string.Join("|", lowItems.Where(i => i.QtyInStock > 0).Select(i => i.Status)));
            T.Check("I03 category, stock and reorder level shown", lowItems.All(i => i.CategoryName == "ZZREG_Cat" && i.ReorderLevel == 5));

            var recentGrid = T.F<DataGridView>(dash, "dgvRecent");
            List<RecentActivityItem> recent = recentGrid.Rows.Cast<DataGridViewRow>().Select(r => (RecentActivityItem)r.DataBoundItem).ToList();
            T.Eq("I05 recent activity capped at 10 (11 records exist)", 10, recent.Count);
            T.Check("I05 recent activity is newest first", recent.Zip(recent.Skip(1), (x, y) => x.ActivityDate >= y.ActivityDate).All(b => b));
            T.Eq("I05 newest record is 2026-02-02 10:00", new DateTime(2026, 2, 2, 10, 0, 0), recent[0].ActivityDate);
            T.Check("I05 all three activity types present", recent.Select(r => r.ActivityType).Distinct().Count() == 3);
            T.Check("I08 order with NULL customer/employee renders '(no customer) - Confirmed' with its amount",
                recent.Any(r => r.ActivityType == "Order" && r.Description == "(no customer) - Confirmed" && r.QtyOrAmount == "75.00"));
            T.Check("I08 stock-in without supplier renders '(no supplier)'", recent.Any(r => r.ActivityType == "Stock In" && r.Description.EndsWith("(no supplier)")));
            T.Check("I08 stock-out without customer renders '(no customer)'", recent.Any(r => r.ActivityType == "Stock Out" && r.Description.EndsWith("(no customer)")));
            T.Check("I05 empty-state labels hidden while data exists", !T.F<Label>(dash, "lblLowStockEmpty").Visible && !T.F<Label>(dash, "lblRecentEmpty").Visible);

            // refresh picks up a controlled change
            string t0 = T.F<Label>(dash, "lblLastRefreshed").Text;
            T.Check("I06 last-refreshed timestamp present", t0.StartsWith("Last refreshed: 20"), t0);
            T.Exec("INSERT StockIn(ProductID,Quantity,UnitCost,TotalCost,DateIn) VALUES (@p0,7,1,7,'2027-03-01')", f.Ok);
            T.Exec("UPDATE Products SET QtyInStock=QtyInStock+7 WHERE ProductID=@p0", f.Ok);
            T.Eq("I06 dashboard does not change by itself (no background refresh)", "35", Card(dash, "StockIn"));
            System.Threading.Thread.Sleep(1100);
            T.F<Button>(dash, "btnRefresh").PerformClick(); T.Pump();
            T.Eq("I06 Refresh re-queries: Stock In quantity 35 -> 42", "42", Card(dash, "StockIn"));
            T.Eq("I06 Refresh re-queries: units 57 -> 64", "64", Card(dash, "Units"));
            T.Check("I06 Refresh updates the timestamp", T.F<Label>(dash, "lblLastRefreshed").Text != t0);
            T.Eq("I06 new record is now the newest activity", new DateTime(2027, 3, 1),
                ((RecentActivityItem)T.F<DataGridView>(dash, "dgvRecent").Rows[0].DataBoundItem).ActivityDate);

            // navigating away and back reloads
            T.Exec("INSERT StockOut(ProductID,Quantity,UnitPrice,TotalPrice,DateOut) VALUES (@p0,2,1,2,'2027-03-02')", f.Ok);
            T.F<Button>(main, "btnNavCategories").PerformClick(); T.Pump();
            T.F<Button>(main, "btnNavDashboard").PerformClick(); T.Pump();
            T.Eq("I06 selecting Dashboard again reloads (Stock Out 10 -> 12)", "12", Card(dash, "StockOut"));
            main.Close(); T.CloseAllForms();

            // read-only: undo the deliberate fixture changes, then compare everything the dashboard could have touched
            T.Exec("DELETE FROM StockIn WHERE DateIn='2027-03-01'; DELETE FROM StockOut WHERE DateOut='2027-03-02'; UPDATE Products SET QtyInStock=50 WHERE ProductID=@p0", f.Ok);
            string snap2 = T.Snapshot();
            frmMain m2 = OpenMain("Dash Tester", "Admin");
            ucDashboard d2 = T.F<ucDashboard>(m2, "_dashboard");
            for (int i = 0; i < 5; i++) { T.F<Button>(d2, "btnRefresh").PerformClick(); T.Pump(); }
            m2.Close(); T.CloseAllForms();
            T.Eq("I09 loading and refreshing the dashboard modified no table (counts, checksums)", snap2, T.Snapshot());
            T.Eq("I10 the dashboard created no StockOut rows", 4L, T.Count("StockOut"));
            T.Check("I10 ... nor changed stock", T.Stock(f.Ok) == 50 && T.Stock(f.Out) == 0);
        }

        // =====================================================================================
        // Read-only smoke against the REAL development database: numbers are compared with independent SQL.
        public static void RunDevReadOnlySmoke()
        {
            T.Group("I Dashboard");
            var main = OpenMain("Dev Tester", "Admin");
            var dash = T.F<ucDashboard>(main, "_dashboard");
            T.Check("DEV dashboard opens on real data", main.Visible && Card(dash, "Products") != "-");
            T.Eq("DEV Total Products", T.Scalar("SELECT COUNT(*) FROM Products").ToString("N0"), Card(dash, "Products"));
            T.Eq("DEV Total Categories", T.Scalar("SELECT COUNT(*) FROM Categories").ToString("N0"), Card(dash, "Categories"));
            T.Eq("DEV Total Units", T.Scalar("SELECT ISNULL(SUM(QtyInStock),0) FROM Products").ToString("N0"), Card(dash, "Units"));
            T.Eq("DEV Low Stock", T.Scalar("SELECT COUNT(*) FROM Products WHERE QtyInStock<=ReorderLevel").ToString("N0"), Card(dash, "LowStock"));
            T.Eq("DEV Stock In qty", T.Scalar("SELECT ISNULL(SUM(Quantity),0) FROM StockIn").ToString("N0"), Card(dash, "StockIn"));
            T.Eq("DEV Stock Out qty", T.Scalar("SELECT ISNULL(SUM(Quantity),0) FROM StockOut").ToString("N0"), Card(dash, "StockOut"));
            T.Eq("DEV Confirmed orders", T.Scalar("SELECT COUNT(*) FROM Orders WHERE Status='Confirmed'").ToString("N0"), Card(dash, "Confirmed"));
            T.Eq("DEV Pending orders", T.Scalar("SELECT COUNT(*) FROM Orders WHERE Status='Pending'").ToString("N0"), Card(dash, "Pending"));
            main.Close(); T.CloseAllForms();

            T.Group("H Reports");
            var rb = new ReportBLL();
            T.Eq("DEV inventory report rows = Products", (int)T.Scalar("SELECT COUNT(*) FROM Products"), rb.GetInventoryReport().Rows.Count);
            T.Eq("DEV stock in report rows = StockIn", (int)T.Scalar("SELECT COUNT(*) FROM StockIn"), rb.GetStockInReport().Rows.Count);
            T.Eq("DEV stock in report cost = SUM(TotalCost)", T.Dec("SELECT ISNULL(SUM(TotalCost),0) FROM StockIn"), rb.GetStockInReport().TotalCost);
            T.Eq("DEV stock out report rows = StockOut", (int)T.Scalar("SELECT COUNT(*) FROM StockOut"), rb.GetStockOutReport().Rows.Count);
            T.Eq("DEV order report rows = Orders", (int)T.Scalar("SELECT COUNT(*) FROM Orders"), rb.GetOrderReport().Rows.Count);
            T.Eq("DEV order report total = SUM(TotalAmount)", T.Dec("SELECT ISNULL(SUM(TotalAmount),0) FROM Orders"), rb.GetOrderReport().TotalAmount);
        }
    }
}
