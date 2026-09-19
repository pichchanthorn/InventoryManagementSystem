using System.Collections.Generic;
using InventoryManagementSystem.Entity;

namespace InventoryManagementSystem.BLL
{
    public class InventoryReportResult
    {
        public List<InventoryReportRow> Rows { get; set; }
        public int TotalProducts { get; set; }
        public long TotalUnitsInStock { get; set; }
        public int LowStockCount { get; set; }
    }

    public class StockInReportResult
    {
        public List<StockInReportRow> Rows { get; set; }
        public int TransactionCount { get; set; }
        public long TotalQuantity { get; set; }
        public decimal TotalCost { get; set; }
    }

    public class StockOutReportResult
    {
        public List<StockOutReportRow> Rows { get; set; }
        public int TransactionCount { get; set; }
        public long TotalQuantity { get; set; }
        public decimal TotalValue { get; set; }
    }

    public class OrderReportResult
    {
        public List<OrderReportRow> Rows { get; set; }
        public int OrderCount { get; set; }
        public decimal TotalAmount { get; set; }
        public int PendingCount { get; set; }
        public int ConfirmedCount { get; set; }
        public int CancelledCount { get; set; }
    }
}
