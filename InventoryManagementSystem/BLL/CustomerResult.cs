namespace InventoryManagementSystem.BLL
{
    public class CustomerResult
    {
        public bool Success { get; private set; }
        public string Message { get; private set; }

        private CustomerResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public static CustomerResult Ok(string message = null)
        {
            return new CustomerResult(true, message);
        }

        public static CustomerResult Fail(string message)
        {
            return new CustomerResult(false, message);
        }
    }
}
