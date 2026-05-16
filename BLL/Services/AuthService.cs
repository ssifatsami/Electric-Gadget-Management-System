using Electric_Gadget_Management.DAL.Repositories;
using ElectricGadget.Web.Models.Entities;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Electric_Gadget_Management.BLL.Services
{
    public class AuthService
    {
        private readonly UserRepository _userRepository;

        public AuthService()
        {
            _userRepository = new UserRepository();
        }

        public string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public (bool IsSuccess, string Message, User? User) Login(string identifier, string password)
        {
            var user = _userRepository.GetUserByIdOrEmail(identifier);
            if (user == null)
            {
                return (false, "Wrong User ID/Email or Password", null);
            }

            if (user.IsLocked)
            {
                return (false, "Your account is locked due to 3 consecutive failed login attempts.", null);
            }

            if (!user.IsActive)
            {
                return (false, "Your account has been suspended by the administrator.", null);
            }

            string inputHash = HashPassword(password);
            if (user.PasswordHash == inputHash)
            {
                // Login successful
                _userRepository.ResetFailedAttempts(user.UserID);
                return (true, "Login Successful", user);
            }
            else
            {
                // Login failed
                user.FailedAttempts++;
                if (user.FailedAttempts >= 3)
                {
                    user.IsLocked = true;
                    _userRepository.UpdateFailedAttempts(user.UserID, user.FailedAttempts, user.IsLocked);
                    return (false, "Your account is locked due to 3 consecutive failed login attempts.", null);
                }
                else
                {
                    _userRepository.UpdateFailedAttempts(user.UserID, user.FailedAttempts, user.IsLocked);
                    return (false, "Wrong User ID/Email or Password", null);
                }
            }
        }

        public (bool IsSuccess, string Message) Register(User user)
        {
            if (_userRepository.GetUserByIdOrEmail(user.UserID) != null)
            {
                return (false, "User ID already exists.");
            }

            user.PasswordHash = HashPassword(user.Password);
            user.Role = "Customer";
            _userRepository.AddUser(user);
            return (true, "Registration Successful");
        }
    }
}
