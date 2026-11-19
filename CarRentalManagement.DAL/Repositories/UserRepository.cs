
using System;
using System.Data;
using System.Data.SqlClient;
using CarRentalManagement.Models.Entities;

namespace CarRentalManagement.DAL.Repositories
{
    public class UserRepository
    {
        public User GetByUsername(string username)
        {
            User user = null;

            using (SqlConnection connection = DbConnection.GetConnection())
            {
                string query = @"
                    SELECT UserID, Username, PasswordHash,
                           FullName, Role, IsActive
                    FROM Users
                    WHERE Username = @Username";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@Username", SqlDbType.NVarChar, 50)
                        .Value = username;

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new User
                            {
                                UserID = Convert.ToInt32(reader["UserID"]),
                                Username = reader["Username"].ToString(),
                                PasswordHash = reader["PasswordHash"].ToString(),
                                FullName = reader["FullName"].ToString(),
                                Role = reader["Role"].ToString(),
                                IsActive = Convert.ToBoolean(reader["IsActive"])
                            };
                        }
                    }
                }
            }

            return user;
        }
    }
}
