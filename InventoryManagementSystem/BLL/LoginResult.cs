using InventoryManagementSystem.Entity;

namespace InventoryManagementSystem.BLL
{
    public enum LoginStatus
    {
        Success,
        ValidationError,
        InvalidCredentials,
        InactiveUser,
        SystemError
    }

    public class LoginResult
    {
        public LoginStatus Status { get; private set; }
        public string Message { get; private set; }
        public UserEntity User { get; private set; }

        private LoginResult(LoginStatus status, string message, UserEntity user)
        {
            Status = status;
            Message = message;
            User = user;
        }

        public static LoginResult CreateSuccess(UserEntity user)
        {
            return new LoginResult(LoginStatus.Success, null, user);
        }

        public static LoginResult CreateFailure(LoginStatus status, string message)
        {
            return new LoginResult(status, message, null);
        }
    }
}
