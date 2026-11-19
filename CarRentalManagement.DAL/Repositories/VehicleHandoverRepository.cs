using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CarRentalManagement.Models.Entities;

namespace CarRentalManagement.DAL.Repositories
{
    public class VehicleHandoverRepository
    {
        public List<VehicleHandover> GetAll()
        {
            List<VehicleHandover> list = new List<VehicleHandover>();

            using (SqlConnection conn = DbConnection.GetConnection())
            {
                string sql = @"
                    SELECT
                        h.HandoverID,
                        h.ContractID,
                        ct.ContractNumber,
                        cu.FullName AS CustomerName,
                        ca.LicensePlate,
                        h.HandoverType,
                        h.HandoverDate,
                        h.Odometer,
                        h.FuelLevel,
                        h.VehicleCondition,
                        h.Notes,
                        h.UserID,
                        u.FullName AS EmployeeName
                    FROM VehicleHandover h
                    INNER JOIN Contracts ct ON h.ContractID = ct.ContractID
                    INNER JOIN Customers cu ON ct.CustomerID = cu.CustomerID
                    INNER JOIN Cars ca ON ct.CarID = ca.CarID
                    INNER JOIN Users u ON h.UserID = u.UserID
                    ORDER BY h.HandoverDate DESC";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            VehicleHandover item = new VehicleHandover();

                            item.HandoverID = Convert.ToInt32(reader["HandoverID"]);
                            item.ContractID = Convert.ToInt32(reader["ContractID"]);
                            item.ContractNumber = reader["ContractNumber"].ToString();
                            item.CustomerName = reader["CustomerName"].ToString();
                            item.LicensePlate = reader["LicensePlate"].ToString();
                            item.HandoverType = reader["HandoverType"].ToString();
                            item.HandoverDate = Convert.ToDateTime(reader["HandoverDate"]);
                            item.Odometer = Convert.ToInt32(reader["Odometer"]);
                            item.FuelLevel = reader["FuelLevel"].ToString();
                            item.VehicleCondition = reader["VehicleCondition"].ToString();
                            item.Notes = reader["Notes"] == DBNull.Value
                                ? ""
                                : reader["Notes"].ToString();
                            item.UserID = Convert.ToInt32(reader["UserID"]);
                            item.EmployeeName = reader["EmployeeName"].ToString();

                            list.Add(item);
                        }
                    }
                }
            }

            return list;
        }

        public void Add(VehicleHandover handover)
        {
            using (SqlConnection conn = DbConnection.GetConnection())
            {
                conn.Open();

                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        int carID = 0;
                        string contractStatus = "";

                        string checkSql = @"
                            SELECT CarID, Status
                            FROM Contracts
                            WHERE ContractID = @ContractID";

                        using (SqlCommand checkCmd = new SqlCommand(
                            checkSql, conn, transaction))
                        {
                            checkCmd.Parameters.AddWithValue(
                                "@ContractID", handover.ContractID);

                            using (SqlDataReader reader = checkCmd.ExecuteReader())
                            {
                                if (!reader.Read())
                                    throw new Exception("Không tìm thấy hợp đồng!");

                                carID = Convert.ToInt32(reader["CarID"]);
                                contractStatus = reader["Status"].ToString();
                            }
                        }

                        string duplicateSql = @"
                            SELECT COUNT(*)
                            FROM VehicleHandover
                            WHERE ContractID = @ContractID
                            AND HandoverType = @HandoverType";

                        using (SqlCommand duplicateCmd = new SqlCommand(
                            duplicateSql, conn, transaction))
                        {
                            duplicateCmd.Parameters.AddWithValue(
                                "@ContractID", handover.ContractID);

                            duplicateCmd.Parameters.AddWithValue(
                                "@HandoverType", handover.HandoverType);

                            int count = Convert.ToInt32(
                                duplicateCmd.ExecuteScalar());

                            if (count > 0)
                                throw new Exception(
                                    "Hợp đồng này đã có bản ghi " +
                                    handover.HandoverType + "!");
                        }

                        if (handover.HandoverType == "Giao xe")
                        {
                            if (contractStatus != "Draft")
                                throw new Exception(
                                    "Chỉ được giao xe cho hợp đồng ở trạng thái Draft!");

                            string carStatusSql = @"
                                SELECT Status
                                FROM Cars
                                WHERE CarID = @CarID";

                            using (SqlCommand carCmd = new SqlCommand(
                                carStatusSql, conn, transaction))
                            {
                                carCmd.Parameters.AddWithValue("@CarID", carID);

                                string carStatus = Convert.ToString(
                                    carCmd.ExecuteScalar());

                                if (carStatus != "Available")
                                    throw new Exception(
                                        "Xe hiện không ở trạng thái sẵn sàng!");
                            }
                        }
                        else if (handover.HandoverType == "Nhận xe")
                        {
                            if (contractStatus != "Active")
                                throw new Exception(
                                    "Chỉ được nhận xe khi hợp đồng đang Active!");

                            string giaoSql = @"
                                SELECT COUNT(*)
                                FROM VehicleHandover
                                WHERE ContractID = @ContractID
                                AND HandoverType = N'Giao xe'";

                            using (SqlCommand giaoCmd = new SqlCommand(
                                giaoSql, conn, transaction))
                            {
                                giaoCmd.Parameters.AddWithValue(
                                    "@ContractID", handover.ContractID);

                                if (Convert.ToInt32(giaoCmd.ExecuteScalar()) == 0)
                                    throw new Exception(
                                        "Hợp đồng chưa có thông tin giao xe!");
                            }
                        }
                        else
                        {
                            throw new Exception("Loại bàn giao không hợp lệ!");
                        }

                        string insertSql = @"
                            INSERT INTO VehicleHandover
                            (
                                ContractID,
                                HandoverType,
                                HandoverDate,
                                Odometer,
                                FuelLevel,
                                VehicleCondition,
                                Notes,
                                UserID
                            )
                            VALUES
                            (
                                @ContractID,
                                @HandoverType,
                                @HandoverDate,
                                @Odometer,
                                @FuelLevel,
                                @VehicleCondition,
                                @Notes,
                                @UserID
                            )";

                        using (SqlCommand cmd = new SqlCommand(
                            insertSql, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue(
                                "@ContractID", handover.ContractID);

                            cmd.Parameters.AddWithValue(
                                "@HandoverType", handover.HandoverType);

                            cmd.Parameters.AddWithValue(
                                "@HandoverDate", handover.HandoverDate);

                            cmd.Parameters.AddWithValue(
                                "@Odometer", handover.Odometer);

                            cmd.Parameters.AddWithValue(
                                "@FuelLevel", handover.FuelLevel);

                            cmd.Parameters.AddWithValue(
                                "@VehicleCondition", handover.VehicleCondition);

                            cmd.Parameters.AddWithValue(
                                "@Notes", string.IsNullOrWhiteSpace(handover.Notes)
                                    ? (object)DBNull.Value
                                    : handover.Notes);

                            cmd.Parameters.AddWithValue(
                                "@UserID", handover.UserID);

                            cmd.ExecuteNonQuery();
                        }

                        string updateContractSql = @"
                            UPDATE Contracts
                            SET Status = @Status
                            WHERE ContractID = @ContractID";

                        using (SqlCommand cmd = new SqlCommand(
                            updateContractSql, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue(
                                "@Status",
                                handover.HandoverType == "Giao xe"
                                    ? "Active"
                                    : "Completed");

                            cmd.Parameters.AddWithValue(
                                "@ContractID", handover.ContractID);

                            cmd.ExecuteNonQuery();
                        }

                        string updateCarSql = @"
                            UPDATE Cars
                            SET Status = @Status
                            WHERE CarID = @CarID";

                        using (SqlCommand cmd = new SqlCommand(
                            updateCarSql, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue(
                                "@Status",
                                handover.HandoverType == "Giao xe"
                                    ? "Rented"
                                    : "Available");

                            cmd.Parameters.AddWithValue("@CarID", carID);

                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}