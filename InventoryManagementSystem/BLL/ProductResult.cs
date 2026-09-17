namespace InventoryManagementSystem.BLL
{
    public class ProductResult
    {
        public bool Success { get; private set; }
        public string Message { get; private set; }

        private ProductResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public static ProductResult Ok(string message = null)
        {
            return new ProductResult(true, message);
        }

        public static ProductResult Fail(string message)
        {
            return new ProductResult(false, message);
        }
    }
}
