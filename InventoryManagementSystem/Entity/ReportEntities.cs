using System;

namespace InventoryManagementSystem.Entity
{
    /// <summary>
    /// Read-only row models used by the Reports module. None of these map to a table for writing.
    /// </summary>
    public class InventoryReportRow
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public decimal UnitPrice { get; set; }
        public int QtyInStock { get; set; }
        public int ReorderLevel { get; set; }
        public string StockStatus { get; set; }
    }

    public class StockInReportRow
    {
        public int StockInID { get; set; }
        public string ProductName { get; set; }
        public string SupplierName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost { get; set; }
        public DateTime DateIn { get; set; }
        public string Notes { get; set; }
    }

    public class StockOutReportRow
    {
        public int StockOutID { get; set; }
        public string ProductName { get; set; }
        public string CustomerName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime DateOut { get; set; }
        public string Notes { get; set; }
    }

    public class OrderReportRow
    {
        public int OrderID { get; set; }
        public string CustomerName { get; set; }
        public string EmployeeName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
    }
}
