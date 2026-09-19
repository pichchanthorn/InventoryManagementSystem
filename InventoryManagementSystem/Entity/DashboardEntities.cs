using System;

namespace InventoryManagementSystem.Entity
{
    /// <summary>
    /// Read-only models used by the Dashboard. None of these are written back to the database.
    /// </summary>
    public class DashboardSummary
    {
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public long TotalUnitsInStock { get; set; }
        public int LowStockCount { get; set; }
        public long StockInQuantity { get; set; }
        public long StockOutQuantity { get; set; }
        public int ConfirmedOrders { get; set; }
        public int PendingOrders { get; set; }
    }

    public class LowStockItem
    {
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public int QtyInStock { get; set; }
        public int ReorderLevel { get; set; }
        public string Status { get; set; }
    }

    /// <summary>Raw recent-activity row as read from StockIn / StockOut / Orders.</summary>
    public class RecentActivityRow
    {
        public string ActivityType { get; set; }
        public int ReferenceID { get; set; }
        public string Subject { get; set; }
        public string Party { get; set; }
        public string Status { get; set; }
        public DateTime ActivityDate { get; set; }
        public int? Quantity { get; set; }
        public decimal Amount { get; set; }
    }

    public class RecentActivityItem
    {
        public string ActivityType { get; set; }
        public int ReferenceID { get; set; }
        public string Description { get; set; }
        public DateTime ActivityDate { get; set; }
        public string QtyOrAmount { get; set; }
    }
}
