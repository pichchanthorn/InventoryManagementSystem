namespace InventoryManagementSystem.BLL
{
    public class CategoryResult
    {
        public bool Success { get; private set; }
        public string Message { get; private set; }

        private CategoryResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public static CategoryResult Ok(string message = null)
        {
            return new CategoryResult(true, message);
        }

        public static CategoryResult Fail(string message)
        {
            return new CategoryResult(false, message);
        }
    }
}
