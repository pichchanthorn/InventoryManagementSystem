namespace InventoryManagementSystem.BLL
{
    public class EmployeeResult
    {
        public bool Success { get; private set; }
        public string Message { get; private set; }

        private EmployeeResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public static EmployeeResult Ok(string message = null)
        {
            return new EmployeeResult(true, message);
        }

        public static EmployeeResult Fail(string message)
        {
            return new EmployeeResult(false, message);
        }
    }
}
