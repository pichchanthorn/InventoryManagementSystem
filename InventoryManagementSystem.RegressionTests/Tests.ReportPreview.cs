using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;
using InventoryManagementSystem.BLL;
using InventoryManagementSystem.UI;
using InventoryManagementSystem.UI.Reporting;

namespace InventoryManagementSystem.RegressionTests
{
    /// <summary>Group N: report preview / printing (document model, deterministic pagination, PrintDocument, preview form, ucReports wiring).</summary>
    internal static class Tests_ReportPreview
    {
        // ---- helpers --------------------------------------------------------------------------

        // Independent re-statement of the pagination rule (fixed geometry: header 90, table header 32, footer 30, row 22, summary 12+26+18/line).
        private static int ExpectedPages(Rectangle mb, int rows, int summaryLines)
        {
            int avail = mb.Height - 90 - 32 - 30;
            int rpp = Math.Max(1, avail / 22);
            int pages = rows == 0 ? 1 : (rows + rpp - 1) / rpp;
            int lastRows = rows == 0 ? 1 : rows - (pages - 1) * rpp;
            int summary = summaryLines == 0 ? 0 : 12 + 26 + summaryLines * 18;
            if (summary == 0) return pages;
            return lastRows * 22 + summary <= avail ? pages : pages + 1;
        }

        private static readonly Rectangle Letter = new Rectangle(50, 50, 750, 1000);   // 8.5x11 portrait, 0.5in margins

        private static ReportDocument Synthetic(int rows, int summaryLines, bool landscape = false)
        {
            var doc = new ReportDocument("Synthetic", DateTime.Now, "None", landscape);
            doc.Columns.Add(new ReportColumn("ID", 1, ReportAlign.Right));
            doc.Columns.Add(new ReportColumn("Name", 5));
            for (int i = 1; i <= rows; i++) doc.Rows.Add(new[] { i.ToString(), "Row " + i });
            for (int i = 1; i <= summaryLines; i++) doc.SummaryLines.Add("Summary " + i);
            return doc;
        }

        /// <summary>Runs the printer's real PrintPage pipeline with the preview controller (no physical printer) and returns the page count generated.</summary>
        private static int GeneratedPages(ReportPrinter printer, out int printPageEvents)
        {
            int events = 0;
            PrintPageEventHandler count = (s, e) => events++;
            printer.PrintPage += count;
            var controller = new PreviewPrintController();
            PrintController old = printer.PrintController;
            printer.PrintController = controller;
            try
            {
                printer.Print();
                printPageEvents = events;
                return controller.GetPreviewPageInfo().Length;
            }
            finally
            {
                printer.PrintController = old;
                printer.PrintPage -= count;
            }
        }

        private static void CheckPrinter(string label, ReportDocument doc, int expectedRows)
        {
            var printer = new ReportPrinter(doc);
            int expected = ExpectedPages(printer.GetMarginBounds(), doc.Rows.Count, doc.SummaryLines.Count);
            T.Eq(label + ": document rows == report rows", expectedRows, doc.Rows.Count);
            T.Eq(label + ": every row has one cell per column", true, doc.Rows.All(r => r.Length == doc.Columns.Count));
            T.Eq(label + ": PageCount matches the deterministic rule", expected, printer.PageCount);

            if (!printer.PrinterSettings.IsValid)
            {
                T.Check(label + ": no printer installed - PrintPage generation skipped", true);
                return;
            }

            int events;
            int generated = GeneratedPages(printer, out events);
            T.Eq(label + ": PrintPage raised once per page, no exception", printer.PageCount, events);
            T.Eq(label + ": pages actually generated == PageCount", printer.PageCount, generated);
        }

        private static void SelectByText(ComboBox c, string text)
        {
            for (int i = 0; i < c.Items.Count; i++) { c.SelectedIndex = i; if (c.Text == text) return; }
            throw new InvalidOperationException("Combo item not found: " + text);
        }

        private static ReportDocument PageDoc(ucReports page)
        {
            return T.F<ReportDocument>(page, "_currentDocument");
        }

        // =====================================================================================
        public static void Run()
        {
            T.Group("N Report preview & printing");
            var rb = new ReportBLL();

            // ---- N1: pure pagination rule (no database) ---------------------------------------
            T.Eq("N01 0 rows -> 1 page (with summary)", 1, ReportPrinter.Paginate(Synthetic(0, 3), Letter).Count);
            T.Eq("N01 0 rows: page shows the summary", true, ReportPrinter.Paginate(Synthetic(0, 3), Letter)[0].HasSummary);
            T.Eq("N01 1 row -> 1 page", 1, ReportPrinter.Paginate(Synthetic(1, 3), Letter).Count);
            // Letter portrait: 1000 - 152 = 848 available -> 38 rows/page; 3 summary lines need 92
            T.Eq("N02 30 rows + summary fits on one page", 1, ReportPrinter.Paginate(Synthetic(30, 3), Letter).Count);
            T.Eq("N02 38 rows fill page 1; summary moves to an extra page", 2, ReportPrinter.Paginate(Synthetic(38, 3), Letter).Count);
            List<ReportPage> p38 = ReportPrinter.Paginate(Synthetic(38, 3), Letter);
            T.Eq("N02 ... and that extra page holds only the summary", "0|True", p38[1].RowCount + "|" + p38[1].HasSummary);
            T.Eq("N02 39 rows -> 2 pages", 2, ReportPrinter.Paginate(Synthetic(39, 3), Letter).Count);
            T.Eq("N02 76 rows -> 3 pages (2 full + summary page)", 3, ReportPrinter.Paginate(Synthetic(76, 3), Letter).Count);
            T.Eq("N02 no summary lines: 38 rows -> 1 page", 1, ReportPrinter.Paginate(Synthetic(38, 0), Letter).Count);
            List<ReportPage> p100 = ReportPrinter.Paginate(Synthetic(100, 3), Letter);
            T.Eq("N03 100 rows: rows are contiguous and complete, summary only on the last page",
                "100|True|False", p100.Sum(p => p.RowCount) + "|" + p100.Last().HasSummary + "|" + p100.Take(p100.Count - 1).Any(p => p.HasSummary));
            T.Eq("N03 page starts are contiguous", true, Enumerable.Range(1, p100.Count - 1).All(i => p100[i].FirstRow == p100[i - 1].FirstRow + p100[i - 1].RowCount));
            T.Eq("N03 tiny page still yields >= 1 row per page (no infinite loop)", 3, ReportPrinter.Paginate(Synthetic(3, 0), new Rectangle(0, 0, 500, 160)).Count);
            T.Eq("N03 pagination is deterministic (same input, same result)",
                string.Join(",", ReportPrinter.Paginate(Synthetic(100, 3), Letter).Select(p => p.RowCount)),
                string.Join(",", ReportPrinter.Paginate(Synthetic(100, 3), Letter).Select(p => p.RowCount)));

            // ---- A. Inventory ----------------------------------------------------------------
            T.Wipe();
            InventoryReportResult inv0 = rb.GetInventoryReport();
            CheckPrinter("N10 inventory 0 rows", ReportDocumentBuilder.FromInventory(inv0, null), 0);
            T.Eq("N10 inventory empty document shows default filter text", ReportDocumentBuilder.NoFilters,
                ReportDocumentBuilder.FromInventory(inv0, null).FilterDescription);

            int cat = T.NewCategory("ZZREG_PCat");
            int cat2 = T.NewCategory("ZZREG_PCat2");
            T.NewProduct("ZZREG_P0001", cat, 50, 12.5m, 5);
            InventoryReportResult inv1 = rb.GetInventoryReport();
            ReportDocument d1 = ReportDocumentBuilder.FromInventory(inv1, "");
            CheckPrinter("N11 inventory 1 row", d1, 1);
            T.Eq("N11 inventory row formatting (id|name|category|price|stock|reorder|status)",
                "1|ZZREG_P0001|ZZREG_PCat|$12.50|50|5|In Stock", string.Join("|", d1.Rows[0]));
            T.Eq("N11 inventory columns", "Product ID|Product Name|Category|Unit Price (USD)|Current Stock|Reorder Level|Stock Status",
                string.Join("|", d1.Columns.Select(c => c.Header)));
            T.Eq("N11 numeric columns are right aligned (ID, price, stock, reorder)", "R|L|L|R|R|R|L",
                string.Join("|", d1.Columns.Select(c => c.Align == ReportAlign.Right ? "R" : "L")));
            T.Eq("N11 inventory is portrait", false, d1.Landscape);
            T.Eq("N11 summary lines come from the BLL result", "Total products: 1|Total units in stock: 50|Low-stock products: 0",
                string.Join("|", d1.SummaryLines));

            for (int i = 2; i <= 120; i++) T.NewProduct("ZZREG_P" + i.ToString("D4"), i % 2 == 0 ? cat : cat2, i % 7, 3m, 5);
            InventoryReportResult invN = rb.GetInventoryReport();
            ReportDocument dN = ReportDocumentBuilder.FromInventory(invN, "");
            CheckPrinter("N12 inventory 120 rows (multi-page)", dN, 120);
            T.Check("N12 inventory needs more than one page", new ReportPrinter(dN).PageCount > 1);
            ReportDocument dCat = ReportDocumentBuilder.FromInventory(rb.GetInventoryReport(cat2), "Category: ZZREG_PCat2");
            CheckPrinter("N13 inventory category filter", dCat, (int)T.Scalar("SELECT COUNT(*) FROM Products WHERE CategoryID=@p0", cat2));
            ReportDocument dLow = ReportDocumentBuilder.FromInventory(rb.GetInventoryReport(null, ReportBLL.InventoryStatusLowStock), "Stock status: Low Stock");
            CheckPrinter("N13 inventory status filter", dLow, (int)T.Scalar("SELECT COUNT(*) FROM Products WHERE QtyInStock <= ReorderLevel"));

            // long text must be truncated, never break layout / pagination
            var longDoc = ReportDocumentBuilder.FromInventory(inv1, "");
            longDoc.Rows[0][1] = new string('W', 600) + "\r\nsecond line";
            var printerLong = new ReportPrinter(longDoc);
            int evLong; int genLong = printerLong.PrinterSettings.IsValid ? GeneratedPages(printerLong, out evLong) : printerLong.PageCount;
            T.Eq("N14 600-char cell renders without breaking the page count", printerLong.PageCount, genLong);

            // ---- B. Stock In -----------------------------------------------------------------
            T.Wipe();
            CheckPrinter("N20 stock in 0 rows", ReportDocumentBuilder.FromStockIn(rb.GetStockInReport(), null), 0);
            cat = T.NewCategory("ZZREG_SCat");
            int pA = T.NewProduct("ZZREG_ProdA", cat, 0);
            int pB = T.NewProduct("ZZREG_ProdB", cat, 0);
            int s1 = T.NewSupplier("ZZREG_Sup1");
            int s2 = T.NewSupplier("ZZREG_Sup2");
            int c1 = T.NewCustomer("ZZREG_CustA");
            int c2 = T.NewCustomer("ZZREG_CustB");
            int emp = T.NewEmployee("ZZREG_EmpX");

            T.Exec("INSERT StockIn(ProductID,SupplierID,Quantity,UnitCost,TotalCost,DateIn,Notes) VALUES (@p0,@p1,3,2.50,7.50,'2026-03-05 09:30:00','ZZREG-UNIQUE')", pA, s1);
            StockInReportResult si1 = rb.GetStockInReport();
            ReportDocument dsi1 = ReportDocumentBuilder.FromStockIn(si1, "");
            CheckPrinter("N21 stock in 1 row", dsi1, 1);
            T.Eq("N21 stock in row formatting (id|product|supplier|qty|cost|total|date|notes)",
                si1.Rows[0].StockInID + "|ZZREG_ProdA|ZZREG_Sup1|3|$2.50|$7.50|2026-03-05 09:30|ZZREG-UNIQUE", string.Join("|", dsi1.Rows[0]));
            T.Eq("N21 stock in columns", "ID|Product|Supplier|Quantity|Unit Cost (USD)|Total Cost (USD)|Date In|Notes", string.Join("|", dsi1.Columns.Select(c => c.Header)));
            T.Eq("N21 stock in is landscape", true, dsi1.Landscape);

            T.Exec(@"INSERT StockIn(ProductID,SupplierID,Quantity,UnitCost,TotalCost,DateIn,Notes)
                     SELECT TOP (@p2) @p0,@p1,1,2.00,2.00,DATEADD(day, ROW_NUMBER() OVER (ORDER BY (SELECT 1)) % 20, '2026-03-01'),'ZZREG bulk in'
                     FROM sys.all_columns a CROSS JOIN sys.all_columns b", pA, s1, 80);
            T.Exec(@"INSERT StockIn(ProductID,SupplierID,Quantity,UnitCost,TotalCost,DateIn,Notes)
                     SELECT TOP (@p2) @p0,@p1,2,1.00,2.00,'2026-04-01','ZZREG other in'
                     FROM sys.all_columns a CROSS JOIN sys.all_columns b", pB, s2, 6);

            ReportDocument dsiAll = ReportDocumentBuilder.FromStockIn(rb.GetStockInReport(), "");
            CheckPrinter("N22 stock in all (multi-page)", dsiAll, (int)T.Count("StockIn"));
            T.Check("N22 stock in needs more than one page", new ReportPrinter(dsiAll).PageCount > 1);
            CheckPrinter("N23 stock in supplier filter",
                ReportDocumentBuilder.FromStockIn(rb.GetStockInReport(null, s2), "Supplier: ZZREG_Sup2"), 6);
            CheckPrinter("N23 stock in product + date filter",
                ReportDocumentBuilder.FromStockIn(rb.GetStockInReport(pA, null, new DateTime(2026, 3, 5), new DateTime(2026, 3, 10)), "x"),
                (int)T.Scalar("SELECT COUNT(*) FROM StockIn WHERE ProductID=@p0 AND DateIn>='2026-03-05' AND DateIn<'2026-03-11'", pA));
            CheckPrinter("N23 stock in search filter", ReportDocumentBuilder.FromStockIn(rb.GetStockInReport(null, null, null, null, "ZZREG-UNIQUE"), "x"), 1);
            CheckPrinter("N23 stock in filter with no match -> empty document", ReportDocumentBuilder.FromStockIn(rb.GetStockInReport(null, null, null, null, "nothing-here"), "x"), 0);

            // ---- C. Stock Out ----------------------------------------------------------------
            CheckPrinter("N30 stock out 0 rows", ReportDocumentBuilder.FromStockOut(rb.GetStockOutReport(), null), 0);
            T.Exec("INSERT StockOut(ProductID,CustomerID,Quantity,UnitPrice,TotalPrice,DateOut,Notes) VALUES (@p0,@p1,2,4.00,8.00,'2026-03-06 14:05:00','ZZREG-OUT-UNIQUE')", pA, c1);
            StockOutReportResult so1 = rb.GetStockOutReport();
            ReportDocument dso1 = ReportDocumentBuilder.FromStockOut(so1, "");
            CheckPrinter("N31 stock out 1 row", dso1, 1);
            T.Eq("N31 stock out row formatting (id|product|customer|qty|price|total|date|notes)",
                so1.Rows[0].StockOutID + "|ZZREG_ProdA|ZZREG_CustA|2|$4.00|$8.00|2026-03-06 14:05|ZZREG-OUT-UNIQUE", string.Join("|", dso1.Rows[0]));
            T.Eq("N31 stock out columns", "ID|Product|Customer|Quantity|Unit Price (USD)|Total Price (USD)|Date Out|Notes", string.Join("|", dso1.Columns.Select(c => c.Header)));
            T.Eq("N31 stock out is landscape", true, dso1.Landscape);

            T.Exec(@"INSERT StockOut(ProductID,CustomerID,Quantity,UnitPrice,TotalPrice,DateOut,Notes)
                     SELECT TOP (@p2) @p0,@p1,1,5.00,5.00,DATEADD(day, ROW_NUMBER() OVER (ORDER BY (SELECT 1)) % 20, '2026-03-01'),'ZZREG bulk out'
                     FROM sys.all_columns a CROSS JOIN sys.all_columns b", pA, c1, 70);
            T.Exec(@"INSERT StockOut(ProductID,CustomerID,Quantity,UnitPrice,TotalPrice,DateOut,Notes)
                     SELECT TOP (@p2) @p0,@p1,1,5.00,5.00,'2026-04-02','ZZREG other out'
                     FROM sys.all_columns a CROSS JOIN sys.all_columns b", pB, c2, 4);
            CheckPrinter("N32 stock out all (multi-page)", ReportDocumentBuilder.FromStockOut(rb.GetStockOutReport(), ""), (int)T.Count("StockOut"));
            T.Check("N32 stock out needs more than one page", new ReportPrinter(ReportDocumentBuilder.FromStockOut(rb.GetStockOutReport(), "")).PageCount > 1);
            CheckPrinter("N33 stock out customer filter", ReportDocumentBuilder.FromStockOut(rb.GetStockOutReport(null, c2), "x"), 4);
            CheckPrinter("N33 stock out product + customer + date filter",
                ReportDocumentBuilder.FromStockOut(rb.GetStockOutReport(pA, c1, new DateTime(2026, 3, 1), new DateTime(2026, 3, 7)), "x"),
                (int)T.Scalar("SELECT COUNT(*) FROM StockOut WHERE ProductID=@p0 AND CustomerID=@p1 AND DateOut>='2026-03-01' AND DateOut<'2026-03-08'", pA, c1));
            CheckPrinter("N33 stock out search filter", ReportDocumentBuilder.FromStockOut(rb.GetStockOutReport(null, null, null, null, "ZZREG-OUT-UNIQUE"), "x"), 1);

            // ---- D. Orders -------------------------------------------------------------------
            CheckPrinter("N40 orders 0 rows", ReportDocumentBuilder.FromOrders(rb.GetOrderReport(), null), 0);
            int o1 = T.Id("INSERT Orders(CustomerID,EmployeeID,OrderDate,TotalAmount,Status) OUTPUT INSERTED.OrderID VALUES (@p0,@p1,'2026-03-07 10:00:00',123.45,'Pending')", c1, emp);
            OrderReportResult or1 = rb.GetOrderReport();
            ReportDocument dor1 = ReportDocumentBuilder.FromOrders(or1, "");
            CheckPrinter("N41 orders 1 row", dor1, 1);
            T.Eq("N41 order row formatting (id|customer|employee|date|total|status)", o1 + "|ZZREG_CustA|ZZREG_EmpX|2026-03-07 10:00|$123.45|Pending", string.Join("|", dor1.Rows[0]));
            T.Eq("N41 order columns", "ID|Customer|Employee|Order Date|Total Amount (USD)|Status", string.Join("|", dor1.Columns.Select(c => c.Header)));
            T.Eq("N41 orders are landscape", true, dor1.Landscape);

            T.Exec(@"INSERT Orders(CustomerID,EmployeeID,OrderDate,TotalAmount,Status)
                     SELECT TOP (@p2) @p0,@p1,DATEADD(day, ROW_NUMBER() OVER (ORDER BY (SELECT 1)) % 20, '2026-03-01'),10.00,'Confirmed'
                     FROM sys.all_columns a CROSS JOIN sys.all_columns b", c1, emp, 80);
            T.Exec(@"INSERT Orders(CustomerID,EmployeeID,OrderDate,TotalAmount,Status)
                     SELECT TOP (@p2) @p0,@p1,DATEADD(day, ROW_NUMBER() OVER (ORDER BY (SELECT 1)) % 20, '2026-03-01'),20.00,'Cancelled'
                     FROM sys.all_columns a CROSS JOIN sys.all_columns b", c2, emp, 30);
            CheckPrinter("N42 orders all (multi-page)", ReportDocumentBuilder.FromOrders(rb.GetOrderReport(), ""), (int)T.Count("Orders"));
            T.Check("N42 orders need more than one page", new ReportPrinter(ReportDocumentBuilder.FromOrders(rb.GetOrderReport(), "")).PageCount > 1);
            CheckPrinter("N43 orders status filter", ReportDocumentBuilder.FromOrders(rb.GetOrderReport("Cancelled"), "x"), 30);
            CheckPrinter("N43 orders customer filter", ReportDocumentBuilder.FromOrders(rb.GetOrderReport(null, c2), "x"), 30);
            CheckPrinter("N43 orders date filter",
                ReportDocumentBuilder.FromOrders(rb.GetOrderReport(null, null, new DateTime(2026, 3, 5), new DateTime(2026, 3, 9)), "x"),
                (int)T.Scalar("SELECT COUNT(*) FROM Orders WHERE OrderDate>='2026-03-05' AND OrderDate<'2026-03-10'"));
            CheckPrinter("N43 orders search filter (customer name)", ReportDocumentBuilder.FromOrders(rb.GetOrderReport(null, null, null, null, "ZZREG_CustB"), "x"), 30);
            CheckPrinter("N43 orders combined status + customer + date",
                ReportDocumentBuilder.FromOrders(rb.GetOrderReport("Confirmed", c1, new DateTime(2026, 3, 2), new DateTime(2026, 3, 12)), "x"),
                (int)T.Scalar("SELECT COUNT(*) FROM Orders WHERE Status='Confirmed' AND CustomerID=@p0 AND OrderDate>='2026-03-02' AND OrderDate<'2026-03-13'", c1));

            // ---- E. Preview form -------------------------------------------------------------
            ReportDocument multi = ReportDocumentBuilder.FromStockIn(rb.GetStockInReport(), "None");
            using (var form = new frmReportPreview(multi))
            {
                form.Show(); T.Pump();
                int pages = new ReportPrinter(multi).PageCount;
                var lbl = T.F<ToolStripLabel>(form, "lblPage");
                var prev = T.F<ToolStripButton>(form, "btnPrevious");
                var next = T.F<ToolStripButton>(form, "btnNext");
                var zoom = T.F<ToolStripComboBox>(form, "cmbZoom");
                var ppc = T.F<PrintPreviewControl>(form, "ppcPreview");

                T.Eq("N50 preview page count matches printer", pages, form.PageCount);
                T.Eq("N50 starts on page 1 with 'Page 1 of N'", "Page 1 of " + pages, lbl.Text);
                T.Check("N50 Previous disabled on first page, Next enabled", !prev.Enabled && next.Enabled);
                T.Eq("N50 preview uses the same document configuration (landscape)", true, multi.Landscape);

                next.PerformClick(); T.Pump();
                T.Eq("N51 Next -> page 2", "Page 2 of " + pages, lbl.Text);
                T.Check("N51 Previous enabled after moving on", prev.Enabled);
                prev.PerformClick(); T.Pump();
                T.Eq("N51 Previous -> back to page 1", "Page 1 of " + pages, lbl.Text);

                form.GoToPage(pages + 50); T.Pump();
                T.Eq("N52 navigation clamps to the last page", "Page " + pages + " of " + pages, lbl.Text);
                T.Check("N52 Next disabled on last page, Previous enabled", !next.Enabled && prev.Enabled);
                form.GoToPage(-5); T.Pump();
                T.Eq("N52 navigation clamps to the first page", "Page 1 of " + pages, lbl.Text);

                T.Eq("N53 zoom defaults to 'Fit page'", "Fit page", zoom.SelectedItem as string);
                zoom.SelectedItem = "150%"; T.Pump();
                T.Check("N53 zoom 150% applied", !ppc.AutoZoom && Math.Abs(ppc.Zoom - 1.5) < 0.001);
                zoom.SelectedItem = "Fit page"; T.Pump();
                T.Check("N53 'Fit page' re-enables auto zoom", ppc.AutoZoom);
            }

            using (var form = new frmReportPreview(ReportDocumentBuilder.FromInventory(rb.GetInventoryReport(9999999), null)))
            {
                form.Show(); T.Pump();
                T.Eq("N54 empty report previews as 'Page 1 of 1'", "Page 1 of 1", T.F<ToolStripLabel>(form, "lblPage").Text);
                T.Check("N54 both navigation buttons disabled on a single page",
                    !T.F<ToolStripButton>(form, "btnPrevious").Enabled && !T.F<ToolStripButton>(form, "btnNext").Enabled);
            }

            // ---- F. ucReports wiring ---------------------------------------------------------
            var page = T.HostPage<ucReports>();
            var btnPreview = T.F<Button>(page, "btnPreview");
            T.Check("N60 Preview / Print button exists with that caption", btnPreview.Text == "Preview / Print");
            T.Check("N60 inventory report loaded -> button enabled, document built", btnPreview.Enabled && PageDoc(page) != null);
            T.Eq("N60 inventory document title", "Inventory / Stock Report", PageDoc(page).Title);
            T.Eq("N60 inventory document rows == grid rows", T.F<DataGridView>(page, "dgvReport").Rows.Count, PageDoc(page).Rows.Count);
            T.Eq("N60 on-screen summary unchanged in wording",
                "Total products: " + PageDoc(page).Rows.Count + "     Total units in stock: " + T.Scalar("SELECT SUM(QtyInStock) FROM Products").ToString("N0") +
                "     Low-stock products: " + T.Scalar("SELECT COUNT(*) FROM Products WHERE QtyInStock <= ReorderLevel"),
                T.F<Label>(page, "lblSummary").Text);

            T.F<RadioButton>(page, "rdoStockIn").Checked = true; T.Pump();
            SelectByText(T.F<ComboBox>(page, "cmbSupplier"), "ZZREG_Sup2");
            T.F<Button>(page, "btnApply").PerformClick(); T.Pump();
            T.Eq("N61 stock in document rows follow the filter", 6, PageDoc(page).Rows.Count);
            T.Eq("N61 filter description names the supplier", "Supplier: ZZREG_Sup2", PageDoc(page).FilterDescription);
            T.Eq("N61 stock in title", "Stock In Report", PageDoc(page).Title);

            T.F<RadioButton>(page, "rdoStockOut").Checked = true; T.Pump();
            T.Eq("N62 switching report resets the filters -> 'None' description", ReportDocumentBuilder.NoFilters, PageDoc(page).FilterDescription);
            SelectByText(T.F<ComboBox>(page, "cmbCustomer"), "ZZREG_CustB");
            T.F<TextBox>(page, "txtSearch").Text = "other out";
            T.F<Button>(page, "btnApply").PerformClick(); T.Pump();
            T.Eq("N62 stock out document rows follow customer + search", 4, PageDoc(page).Rows.Count);
            T.Eq("N62 filter description lists customer and search", "Customer: ZZREG_CustB; Search: \"other out\"", PageDoc(page).FilterDescription);

            T.F<RadioButton>(page, "rdoOrders").Checked = true; T.Pump();
            var chk = T.F<CheckBox>(page, "chkDateFilter");
            chk.Checked = true;
            T.F<DateTimePicker>(page, "dtpFrom").Value = new DateTime(2026, 3, 5);
            T.F<DateTimePicker>(page, "dtpTo").Value = new DateTime(2026, 3, 9);
            SelectByText(T.F<ComboBox>(page, "cmbStatus"), "Confirmed");
            T.F<Button>(page, "btnApply").PerformClick(); T.Pump();
            T.Eq("N63 orders document rows follow status + date",
                (int)T.Scalar("SELECT COUNT(*) FROM Orders WHERE Status='Confirmed' AND OrderDate>='2026-03-05' AND OrderDate<'2026-03-10'"), PageDoc(page).Rows.Count);
            T.Eq("N63 filter description lists status and date range", "Status: Confirmed; Date: 2026-03-05 to 2026-03-09", PageDoc(page).FilterDescription);

            // switching report type discards the previous report's document until the new one has loaded
            T.F<RadioButton>(page, "rdoInventory").Checked = true; T.Pump();
            T.Eq("N64 switching to Inventory -> document is the inventory report", "Inventory / Stock Report", PageDoc(page).Title);
            T.ClosePage(page);
        }
    }
}
