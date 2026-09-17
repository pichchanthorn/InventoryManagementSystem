using System;

namespace InventoryManagementSystem.Entity
{
    public class ProductEntity
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int CategoryID { get; set; }
        public decimal UnitPrice { get; set; }
        public int QtyInStock { get; set; }
        public string Description { get; set; }
        public string Barcode { get; set; }
        public int ReorderLevel { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Display-only; populated by a join in ProductDAL.GetAll(). Not a database column.
        /// </summary>
        public string CategoryName { get; set; }
    }
}
