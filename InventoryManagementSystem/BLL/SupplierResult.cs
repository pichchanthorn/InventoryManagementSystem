namespace InventoryManagementSystem.BLL
{
    public class SupplierResult
    {
        public bool Success { get; private set; }
        public string Message { get; private set; }

        private SupplierResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public static SupplierResult Ok(string message = null)
        {
            return new SupplierResult(true, message);
        }

        public static SupplierResult Fail(string message)
        {
            return new SupplierResult(false, message);
        }
    }
}
