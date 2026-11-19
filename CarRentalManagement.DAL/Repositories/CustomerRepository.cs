
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CarRentalManagement.Models.Entities;

namespace CarRentalManagement.DAL.Repositories
{
    public class CustomerRepository
    {
        // Lấy danh sách tất cả khách hàng
        public List<Customer> GetAll()
        {
            List<Customer> customers = new List<Customer>();

            string query = @"
                SELECT CustomerID, FullName, Phone, Email,
                       CitizenID, DriverLicense, Address
                FROM Customers";

            using (SqlConnection connection = DbConnection.GetConnection())
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Customer customer = new Customer
                        {
                            CustomerID = Convert.ToInt32(reader["CustomerID"]),
                            FullName = reader["FullName"].ToString(),
                            Phone = reader["Phone"].ToString(),
                            Email = reader["Email"] == DBNull.Value
                                ? "" : reader["Email"].ToString(),
                            CitizenID = reader["CitizenID"].ToString(),
                            DriverLicense = reader["DriverLicense"] == DBNull.Value
                                ? "" : reader["DriverLicense"].ToString(),
                            Address = reader["Address"] == DBNull.Value
                                ? "" : reader["Address"].ToString()
                        };

                        customers.Add(customer);
                    }
                }
            }

            return customers;
        }

        // Thêm khách hàng
        public bool Add(Customer customer)
        {
            string query = @"
                INSERT INTO Customers
                (FullName, Phone, Email, CitizenID, DriverLicense, Address)
                VALUES
                (@FullName, @Phone, @Email, @CitizenID, @DriverLicense, @Address)";

            using (SqlConnection connection = DbConnection.GetConnection())
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@FullName", customer.FullName);
                command.Parameters.AddWithValue("@Phone", customer.Phone);
                command.Parameters.AddWithValue("@Email",
                    string.IsNullOrWhiteSpace(customer.Email)
                        ? (object)DBNull.Value : customer.Email);
                command.Parameters.AddWithValue("@CitizenID", customer.CitizenID);
                command.Parameters.AddWithValue("@DriverLicense",
                    string.IsNullOrWhiteSpace(customer.DriverLicense)
                        ? (object)DBNull.Value : customer.DriverLicense);
                command.Parameters.AddWithValue("@Address",
                    string.IsNullOrWhiteSpace(customer.Address)
                        ? (object)DBNull.Value : customer.Address);

                connection.Open();

                return command.ExecuteNonQuery() > 0;
            }
        }

        // Cập nhật khách hàng
        public bool Update(Customer customer)
        {
            string query = @"
                UPDATE Customers
                SET FullName = @FullName,
                    Phone = @Phone,
                    Email = @Email,
                    CitizenID = @CitizenID,
                    DriverLicense = @DriverLicense,
                    Address = @Address
                WHERE CustomerID = @CustomerID";

            using (SqlConnection connection = DbConnection.GetConnection())
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@CustomerID", customer.CustomerID);
                command.Parameters.AddWithValue("@FullName", customer.FullName);
                command.Parameters.AddWithValue("@Phone", customer.Phone);
                command.Parameters.AddWithValue("@Email",
                    string.IsNullOrWhiteSpace(customer.Email)
                        ? (object)DBNull.Value : customer.Email);
                command.Parameters.AddWithValue("@CitizenID", customer.CitizenID);
                command.Parameters.AddWithValue("@DriverLicense",
                    string.IsNullOrWhiteSpace(customer.DriverLicense)
                        ? (object)DBNull.Value : customer.DriverLicense);
                command.Parameters.AddWithValue("@Address",
                    string.IsNullOrWhiteSpace(customer.Address)
                        ? (object)DBNull.Value : customer.Address);

                connection.Open();

                return command.ExecuteNonQuery() > 0;
            }
        }

        // Xóa khách hàng
        public bool Delete(int customerID)
        {
            string query = @"
                DELETE FROM Customers
                WHERE CustomerID = @CustomerID";

            using (SqlConnection connection = DbConnection.GetConnection())
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@CustomerID", customerID);

                connection.Open();

                return command.ExecuteNonQuery() > 0;
            }
        }
    }
}
