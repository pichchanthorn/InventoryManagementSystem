namespace InventoryManagementSystem.BLL
{
    public class OrderResult
    {
        public bool Success { get; private set; }
        public string Message { get; private set; }
        public int OrderID { get; private set; }

        private OrderResult(bool success, string message, int orderId)
        {
            Success = success;
            Message = message;
            OrderID = orderId;
        }

        public static OrderResult Ok(string message = null, int orderId = 0)
        {
            return new OrderResult(true, message, orderId);
        }

        public static OrderResult Fail(string message)
        {
            return new OrderResult(false, message, 0);
        }
    }
}
