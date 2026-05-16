using Electric_Gadget_Management.DAL.Database;
using ElectricGadget.Web.Models.Entities;
using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace Electric_Gadget_Management.DAL.Repositories
{
    public class UserRepository
    {
        private readonly DatabaseHelper _dbHelper;

        public UserRepository()
        {
            _dbHelper = new DatabaseHelper();
        }

        public User? GetUserByIdOrEmail(string identifier)
        {
            string query = "SELECT UserID, Name, Email, PasswordHash, Role, IsLocked, FailedAttempts, CreatedAt FROM Users WHERE UserID = @ID OR Email = @ID";
            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@ID", identifier) };

            DataTable dt = _dbHelper.ExecuteQuery(query, parameters);
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                return new User
                {
                    UserID = row["UserID"]?.ToString() ?? "",
                    Name = row["Name"]?.ToString() ?? "",
                    Email = row["Email"]?.ToString(),
                    PasswordHash = row["PasswordHash"]?.ToString(),
                    Role = row["Role"]?.ToString() ?? "Customer",
                    IsLocked = Convert.ToBoolean(row["IsLocked"]),
                    FailedAttempts = Convert.ToInt32(row["FailedAttempts"]),
                    CreatedAt = Convert.ToDateTime(row["CreatedAt"])
                };
            }
            return null;
        }

        public void UpdateFailedAttempts(string userId, int attempts, bool isLocked)
        {
            string query = "UPDATE Users SET FailedAttempts = @FailedAttempts, IsLocked = @IsLocked WHERE UserID = @UserID";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@FailedAttempts", attempts),
                new SqlParameter("@IsLocked", isLocked),
                new SqlParameter("@UserID", userId)
            };
            _dbHelper.ExecuteNonQuery(query, parameters);
        }

        public void ResetFailedAttempts(string userId)
        {
            string query = "UPDATE Users SET FailedAttempts = 0, IsLocked = 0 WHERE UserID = @UserID";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UserID", userId)
            };
            _dbHelper.ExecuteNonQuery(query, parameters);
        }

        public void AddUser(User user)
        {
            string query = "INSERT INTO Users (UserID, Name, Email, Password, PasswordHash, Role, IsLocked, FailedAttempts, CreatedAt) " +
                           "VALUES (@UserID, @Name, @Email, @Password, @PasswordHash, @Role, @IsLocked, @FailedAttempts, @CreatedAt)";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UserID", user.UserID),
                new SqlParameter("@Name", user.Name),
                new SqlParameter("@Email", (object?)user.Email ?? DBNull.Value),
                new SqlParameter("@Password", user.Password),
                new SqlParameter("@PasswordHash", user.PasswordHash),
                new SqlParameter("@Role", user.Role),
                new SqlParameter("@IsLocked", user.IsLocked),
                new SqlParameter("@FailedAttempts", user.FailedAttempts),
                new SqlParameter("@CreatedAt", user.CreatedAt)
            };
            _dbHelper.ExecuteNonQuery(query, parameters);
        }
    }
}
