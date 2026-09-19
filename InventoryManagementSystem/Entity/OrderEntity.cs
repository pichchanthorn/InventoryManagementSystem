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

        /// <summary>
        /// Display-only; populated by a join in OrderDAL. Not a database column.
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// Display-only; populated by a join in OrderDAL. Not a database column.
        /// </summary>
        public string EmployeeName { get; set; }
    }
}
