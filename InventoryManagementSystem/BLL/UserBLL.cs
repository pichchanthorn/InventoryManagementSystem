using System;
using System.Data.SqlClient;
using InventoryManagementSystem.DAL;
using InventoryManagementSystem.Entity;

namespace InventoryManagementSystem.BLL
{
    public class UserBLL
    {
        public LoginResult Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return LoginResult.CreateFailure(LoginStatus.ValidationError,
                    "Please enter both username and password.");
            }

            UserEntity user;
            try
            {
                user = UserDAL.GetByUsername(username.Trim());
            }
            catch (SqlException)
            {
                return LoginResult.CreateFailure(LoginStatus.SystemError,
                    "Unable to reach the database. Please try again later.");
            }
            catch (InvalidOperationException)
            {
                return LoginResult.CreateFailure(LoginStatus.SystemError,
                    "Unable to reach the database. Please try again later.");
            }

            if (user == null || !PasswordHasher.VerifyPassword(password, user.PasswordHash))
            {
                return LoginResult.CreateFailure(LoginStatus.InvalidCredentials,
                    "Invalid username or password.");
            }

            if (!user.IsActive)
            {
                return LoginResult.CreateFailure(LoginStatus.InactiveUser,
                    "This account is inactive. Please contact the administrator.");
            }

            return LoginResult.CreateSuccess(user);
        }
    }
}
