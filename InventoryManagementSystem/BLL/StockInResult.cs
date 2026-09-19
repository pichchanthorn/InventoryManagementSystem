namespace InventoryManagementSystem.BLL
{
    public class StockInResult
    {
        public bool Success { get; private set; }
        public string Message { get; private set; }

        private StockInResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public static StockInResult Ok(string message = null)
        {
            return new StockInResult(true, message);
        }

        public static StockInResult Fail(string message)
        {
            return new StockInResult(false, message);
        }
    }
}
