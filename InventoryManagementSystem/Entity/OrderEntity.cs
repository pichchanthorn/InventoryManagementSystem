using System;

namespace InventoryManagementSystem.Entity
{
    public class OrderEntity
    {
        public int OrderID { get; set; }
        public int? CustomerID { get; set; }
        public int? EmployeeID { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
    }
}
