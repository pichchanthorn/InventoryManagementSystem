namespace InventoryManagementSystem.BLL
{
    public class StockOutResult
    {
        public bool Success { get; private set; }
        public string Message { get; private set; }

        private StockOutResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public static StockOutResult Ok(string message = null)
        {
            return new StockOutResult(true, message);
        }

        public static StockOutResult Fail(string message)
        {
            return new StockOutResult(false, message);
        }
    }
}
