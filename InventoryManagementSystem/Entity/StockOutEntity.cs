using System;

namespace InventoryManagementSystem.Entity
{
    public class StockOutEntity
    {
        public int StockOutID { get; set; }
        public int ProductID { get; set; }
        public int? CustomerID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime DateOut { get; set; }
        public string Notes { get; set; }
    }
}
