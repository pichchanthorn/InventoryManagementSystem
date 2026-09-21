using System;
using System.Collections.Generic;
using InventoryManagementSystem.BLL;
using InventoryManagementSystem.Entity;

namespace InventoryManagementSystem.UI.Reporting
{
    /// <summary>
    /// Maps the BLL report results to a <see cref="ReportDocument"/>. Only formats values for display; every
    /// total and count comes from the BLL result, so the printed report always matches the grid.
    /// </summary>
    public static class ReportDocumentBuilder
    {
        /// <summary>USD presentation for printed/previewed values, e.g. $1,250.00. Display only - the numbers stay decimals.</summary>
        private static string Usd(decimal value)
        {
            return "$" + value.ToString("#,##0.00", System.Globalization.CultureInfo.InvariantCulture);
        }

        private const string DateTimeFormat = "yyyy-MM-dd HH:mm";
        public const string NoFilters = "None (all records)";

        public static ReportDocument FromInventory(InventoryReportResult result, string filterDescription)
        {
            var doc = Create("Inventory / Stock Report", filterDescription, false);
            doc.Columns.Add(new ReportColumn("Product ID", 9, ReportAlign.Right));
            doc.Columns.Add(new ReportColumn("Product Name", 22));
            doc.Columns.Add(new ReportColumn("Category", 16));
            doc.Columns.Add(new ReportColumn("Unit Price (USD)", 15, ReportAlign.Right));
            doc.Columns.Add(new ReportColumn("Current Stock", 11, ReportAlign.Right));
            doc.Columns.Add(new ReportColumn("Reorder Level", 11, ReportAlign.Right));
            doc.Columns.Add(new ReportColumn("Stock Status", 12));

            foreach (InventoryReportRow r in result.Rows)
                doc.Rows.Add(new[]
                {
                    r.ProductID.ToString(), Clean(r.ProductName), Clean(r.CategoryName), Usd(r.UnitPrice),
                    r.QtyInStock.ToString(), r.ReorderLevel.ToString(), Clean(r.StockStatus)
                });

            doc.SummaryLines.AddRange(InventorySummary(result));
            return doc;
        }

        public static ReportDocument FromStockIn(StockInReportResult result, string filterDescription)
        {
            var doc = Create("Stock In Report", filterDescription, true);
            doc.Columns.Add(new ReportColumn("ID", 6, ReportAlign.Right));
            doc.Columns.Add(new ReportColumn("Product", 22));
            doc.Columns.Add(new ReportColumn("Supplier", 18));
            doc.Columns.Add(new ReportColumn("Quantity", 9, ReportAlign.Right));
            doc.Columns.Add(new ReportColumn("Unit Cost (USD)", 14, ReportAlign.Right));
            doc.Columns.Add(new ReportColumn("Total Cost (USD)", 15, ReportAlign.Right));
            doc.Columns.Add(new ReportColumn("Date In", 14));
            doc.Columns.Add(new ReportColumn("Notes", 20));

            foreach (StockInReportRow r in result.Rows)
                doc.Rows.Add(new[]
                {
                    r.StockInID.ToString(), Clean(r.ProductName), Clean(r.SupplierName), r.Quantity.ToString(),
                    Usd(r.UnitCost), Usd(r.TotalCost), r.DateIn.ToString(DateTimeFormat), Clean(r.Notes)
                });

            doc.SummaryLines.AddRange(StockInSummary(result));
            return doc;
        }

        public static ReportDocument FromStockOut(StockOutReportResult result, string filterDescription)
        {
            var doc = Create("Stock Out Report", filterDescription, true);
            doc.Columns.Add(new ReportColumn("ID", 6, ReportAlign.Right));
            doc.Columns.Add(new ReportColumn("Product", 22));
            doc.Columns.Add(new ReportColumn("Customer", 18));
            doc.Columns.Add(new ReportColumn("Quantity", 9, ReportAlign.Right));
            doc.Columns.Add(new ReportColumn("Unit Value (USD)", 14, ReportAlign.Right));
            doc.Columns.Add(new ReportColumn("Total Value (USD)", 15, ReportAlign.Right));
            doc.Columns.Add(new ReportColumn("Date Out", 14));
            doc.Columns.Add(new ReportColumn("Reason / Notes", 20));

            foreach (StockOutReportRow r in result.Rows)
                doc.Rows.Add(new[]
                {
                    r.StockOutID.ToString(), Clean(r.ProductName), Clean(r.CustomerName), r.Quantity.ToString(),
                    Usd(r.UnitPrice), Usd(r.TotalPrice), r.DateOut.ToString(DateTimeFormat), Clean(r.Notes)
                });

            doc.SummaryLines.AddRange(StockOutSummary(result));
            return doc;
        }

        public static ReportDocument FromOrders(OrderReportResult result, string filterDescription)
        {
            var doc = Create("Orders Report", filterDescription, true);
            doc.Columns.Add(new ReportColumn("ID", 7, ReportAlign.Right));
            doc.Columns.Add(new ReportColumn("Customer", 25));
            doc.Columns.Add(new ReportColumn("Employee", 22));
            doc.Columns.Add(new ReportColumn("Order Date", 16));
            doc.Columns.Add(new ReportColumn("Total Amount (USD)", 17, ReportAlign.Right));
            doc.Columns.Add(new ReportColumn("Status", 12));

            foreach (OrderReportRow r in result.Rows)
                doc.Rows.Add(new[]
                {
                    r.OrderID.ToString(), Clean(r.CustomerName), Clean(r.EmployeeName), r.OrderDate.ToString(DateTimeFormat),
                    Usd(r.TotalAmount), Clean(r.Status)
                });

            doc.SummaryLines.AddRange(OrderSummary(result));
            return doc;
        }

        // Summary lines are shared with the on-screen summary bar (ucReports joins them with spaces).
        public static List<string> InventorySummary(InventoryReportResult r)
        {
            return new List<string>
            {
                "Total products: " + r.TotalProducts,
                "Total units in stock: " + r.TotalUnitsInStock.ToString("N0"),
                "Low-stock products: " + r.LowStockCount
            };
        }

        public static List<string> StockInSummary(StockInReportResult r)
        {
            return new List<string>
            {
                "Stock In transactions: " + r.TransactionCount,
                "Total quantity received: " + r.TotalQuantity.ToString("N0"),
                "Total cost: " + Usd(r.TotalCost)
            };
        }

        public static List<string> StockOutSummary(StockOutReportResult r)
        {
            return new List<string>
            {
                "Stock Out transactions: " + r.TransactionCount,
                "Total quantity issued: " + r.TotalQuantity.ToString("N0"),
                "Total value: " + Usd(r.TotalValue)
            };
        }

        public static List<string> OrderSummary(OrderReportResult r)
        {
            return new List<string>
            {
                "Orders: " + r.OrderCount,
                "Total order amount: " + Usd(r.TotalAmount),
                "Pending: " + r.PendingCount,
                "Confirmed: " + r.ConfirmedCount,
                "Cancelled: " + r.CancelledCount
            };
        }

        private static ReportDocument Create(string title, string filterDescription, bool landscape)
        {
            return new ReportDocument(title, DateTime.Now,
                string.IsNullOrWhiteSpace(filterDescription) ? NoFilters : filterDescription, landscape);
        }

        // One printed row is always one line: line breaks in free text (such as notes) become spaces.
        private static string Clean(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;
            return value.Replace("\r\n", " ").Replace('\r', ' ').Replace('\n', ' ').Replace('\t', ' ').Trim();
        }
    }
}
