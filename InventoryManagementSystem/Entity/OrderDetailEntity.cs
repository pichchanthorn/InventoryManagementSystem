namespace InventoryManagementSystem.Entity
{
    public class OrderDetailEntity
    {
        public int OrderDetailID { get; set; }
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }

        /// <summary>
        /// Display-only; populated by a join in OrderDAL. Not a database column.
        /// </summary>
        public string ProductName { get; set; }
    }
}
