using System;
using System.Data.SqlClient;
using System.Linq;
using InventoryManagementSystem.DAL;

namespace InventoryManagementSystem.BLL
{
    /// <summary>
    /// Read-only reporting. Summaries are computed from the same filtered rows the grid displays,
    /// so totals always match what the user sees.
    /// </summary>
    public class ReportBLL
    {
        public const string InventoryStatusInStock = ReportDAL.StatusInStock;
        public const string InventoryStatusLowStock = ReportDAL.StatusLowStock;

        public InventoryReportResult GetInventoryReport(int? categoryId = null, string stockStatus = null,
            string searchText = null)
        {
            return Execute("inventory report", () =>
            {
                var rows = ReportDAL.GetInventory(categoryId, NormalizeOptional(stockStatus), NormalizeOptional(searchText));
                return new InventoryReportResult
                {
                    Rows = rows,
                    TotalProducts = rows.Count,
                    TotalUnitsInStock = rows.Sum(r => (long)r.QtyInStock),
                    LowStockCount = rows.Count(r => r.QtyInStock <= r.ReorderLevel)
                };
            });
        }

        public StockInReportResult GetStockInReport(int? productId = null, int? supplierId = null,
            DateTime? dateFrom = null, DateTime? dateTo = null, string searchText = null)
        {
            ValidateDateRange(dateFrom, dateTo);

            return Execute("stock in report", () =>
            {
                var rows = ReportDAL.GetStockIn(productId, supplierId, StartOfDay(dateFrom), EndExclusive(dateTo),
                    NormalizeOptional(searchText));
                return new StockInReportResult
                {
                    Rows = rows,
                    TransactionCount = rows.Count,
                    TotalQuantity = rows.Sum(r => (long)r.Quantity),
                    TotalCost = rows.Sum(r => r.TotalCost)
                };
            });
        }

        public StockOutReportResult GetStockOutReport(int? productId = null, int? customerId = null,
            DateTime? dateFrom = null, DateTime? dateTo = null, string searchText = null)
        {
            ValidateDateRange(dateFrom, dateTo);

            return Execute("stock out report", () =>
            {
                var rows = ReportDAL.GetStockOut(productId, customerId, StartOfDay(dateFrom), EndExclusive(dateTo),
                    NormalizeOptional(searchText));
                return new StockOutReportResult
                {
                    Rows = rows,
                    TransactionCount = rows.Count,
                    TotalQuantity = rows.Sum(r => (long)r.Quantity),
                    TotalValue = rows.Sum(r => r.TotalPrice)
                };
            });
        }

        public OrderReportResult GetOrderReport(string status = null, int? customerId = null,
            DateTime? dateFrom = null, DateTime? dateTo = null, string searchText = null)
        {
            ValidateDateRange(dateFrom, dateTo);

            return Execute("order report", () =>
            {
                var rows = ReportDAL.GetOrders(NormalizeOptional(status), customerId, StartOfDay(dateFrom),
                    EndExclusive(dateTo), NormalizeOptional(searchText));
                return new OrderReportResult
                {
                    Rows = rows,
                    OrderCount = rows.Count,
                    TotalAmount = rows.Sum(r => r.TotalAmount),
                    PendingCount = rows.Count(r => r.Status == "Pending"),
                    ConfirmedCount = rows.Count(r => r.Status == "Confirmed"),
                    CancelledCount = rows.Count(r => r.Status == "Cancelled")
                };
            });
        }

        private static T Execute<T>(string reportName, Func<T> query)
        {
            try
            {
                return query();
            }
            catch (SqlException ex)
            {
                throw new ApplicationException(
                    "Unable to load the " + reportName + ". Please check your database connection and try again.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new ApplicationException(
                    "Unable to load the " + reportName + ". Please check your database connection and try again.", ex);
            }
        }

        private static void ValidateDateRange(DateTime? dateFrom, DateTime? dateTo)
        {
            if (dateFrom.HasValue && dateTo.HasValue && dateFrom.Value.Date > dateTo.Value.Date)
                throw new ArgumentException("The 'From' date cannot be later than the 'To' date.");
        }

        private static DateTime? StartOfDay(DateTime? date)
        {
            return date.HasValue ? date.Value.Date : (DateTime?)null;
        }

        // Inclusive To date: filter with "< the following midnight" so time components never exclude rows.
        private static DateTime? EndExclusive(DateTime? date)
        {
            return date.HasValue ? date.Value.Date.AddDays(1) : (DateTime?)null;
        }

        private static string NormalizeOptional(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }
}
