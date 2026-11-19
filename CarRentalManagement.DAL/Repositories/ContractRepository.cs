
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CarRentalManagement.Models.Entities;

namespace CarRentalManagement.DAL.Repositories
{
    public class ContractRepository
    {
        // Lấy danh sách hợp đồng
        public List<Contract> GetAll()
        {
            List<Contract> contracts = new List<Contract>();

            string query = @"
                SELECT ContractID, ContractNumber, CustomerID,
                       CarID, UserID, StartDate, ExpectedReturnDate,
                       DailyRate, Deposit, Status, Notes
                FROM Contracts
                ORDER BY ContractID DESC";

            using (SqlConnection connection = DbConnection.GetConnection())
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Contract contract = new Contract
                        {
                            ContractID = Convert.ToInt32(reader["ContractID"]),
                            ContractNumber = reader["ContractNumber"].ToString(),
                            CustomerID = Convert.ToInt32(reader["CustomerID"]),
                            CarID = Convert.ToInt32(reader["CarID"]),
                            UserID = Convert.ToInt32(reader["UserID"]),
                            StartDate = Convert.ToDateTime(reader["StartDate"]),
                            ExpectedReturnDate = Convert.ToDateTime(reader["ExpectedReturnDate"]),
                            DailyRate = Convert.ToDecimal(reader["DailyRate"]),
                            Deposit = Convert.ToDecimal(reader["Deposit"]),
                            Status = reader["Status"].ToString(),
                            Notes = reader["Notes"] == DBNull.Value
                                ? ""
                                : reader["Notes"].ToString()
                        };

                        contracts.Add(contract);
                    }
                }
            }

            return contracts;
        }

        // Thêm hợp đồng
        public bool Add(Contract contract)
        {
            string query = @"
                INSERT INTO Contracts
                (ContractNumber, CustomerID, CarID, UserID,
                 StartDate, ExpectedReturnDate, DailyRate,
                 Deposit, Status, Notes)
                VALUES
                (@ContractNumber, @CustomerID, @CarID, @UserID,
                 @StartDate, @ExpectedReturnDate, @DailyRate,
                 @Deposit, @Status, @Notes)";

            using (SqlConnection connection = DbConnection.GetConnection())
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ContractNumber", contract.ContractNumber);
                command.Parameters.AddWithValue("@CustomerID", contract.CustomerID);
                command.Parameters.AddWithValue("@CarID", contract.CarID);
                command.Parameters.AddWithValue("@UserID", contract.UserID);
                command.Parameters.AddWithValue("@StartDate", contract.StartDate);
                command.Parameters.AddWithValue("@ExpectedReturnDate", contract.ExpectedReturnDate);
                command.Parameters.AddWithValue("@DailyRate", contract.DailyRate);
                command.Parameters.AddWithValue("@Deposit", contract.Deposit);
                command.Parameters.AddWithValue("@Status", contract.Status);

                command.Parameters.AddWithValue("@Notes",
                    string.IsNullOrWhiteSpace(contract.Notes)
                        ? (object)DBNull.Value
                        : contract.Notes);

                connection.Open();

                return command.ExecuteNonQuery() > 0;
            }
        }

        // Cập nhật hợp đồng
        public bool Update(Contract contract)
        {
            string query = @"
                UPDATE Contracts
                SET ContractNumber = @ContractNumber,
                    CustomerID = @CustomerID,
                    CarID = @CarID,
                    UserID = @UserID,
                    StartDate = @StartDate,
                    ExpectedReturnDate = @ExpectedReturnDate,
                    DailyRate = @DailyRate,
                    Deposit = @Deposit,
                    Status = @Status,
                    Notes = @Notes
                WHERE ContractID = @ContractID";

            using (SqlConnection connection = DbConnection.GetConnection())
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ContractID", contract.ContractID);
                command.Parameters.AddWithValue("@ContractNumber", contract.ContractNumber);
                command.Parameters.AddWithValue("@CustomerID", contract.CustomerID);
                command.Parameters.AddWithValue("@CarID", contract.CarID);
                command.Parameters.AddWithValue("@UserID", contract.UserID);
                command.Parameters.AddWithValue("@StartDate", contract.StartDate);
                command.Parameters.AddWithValue("@ExpectedReturnDate", contract.ExpectedReturnDate);
                command.Parameters.AddWithValue("@DailyRate", contract.DailyRate);
                command.Parameters.AddWithValue("@Deposit", contract.Deposit);
                command.Parameters.AddWithValue("@Status", contract.Status);

                command.Parameters.AddWithValue("@Notes",
                    string.IsNullOrWhiteSpace(contract.Notes)
                        ? (object)DBNull.Value
                        : contract.Notes);

                connection.Open();

                return command.ExecuteNonQuery() > 0;
            }
        }

        // Xóa hợp đồng
        public bool Delete(int contractID)
        {
            string query = @"
                DELETE FROM Contracts
                WHERE ContractID = @ContractID";

            using (SqlConnection connection = DbConnection.GetConnection())
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ContractID", contractID);

                connection.Open();

                return command.ExecuteNonQuery() > 0;
            }
        }
    }
}
