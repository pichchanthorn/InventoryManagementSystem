using System;

namespace InventoryManagementSystem.Entity
{
    public class StockInEntity
    {
        public int StockInID { get; set; }
        public int ProductID { get; set; }
        public int? SupplierID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost { get; set; }
        public DateTime DateIn { get; set; }
        public string Notes { get; set; }
    }
}
