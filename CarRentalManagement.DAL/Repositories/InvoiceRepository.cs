
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CarRentalManagement.Models.Entities;

namespace CarRentalManagement.DAL.Repositories
{
    public class InvoiceRepository
    {
        // Lấy toàn bộ danh sách hóa đơn
        public List<Invoice> GetAll()
        {
            List<Invoice> invoices = new List<Invoice>();

            using (SqlConnection conn = DbConnection.GetConnection())
            {
                string query = @"
                    SELECT
                        i.InvoiceID,
                        i.InvoiceNumber,
                        i.ContractID,
                        c.ContractNumber,
                        cu.FullName AS CustomerName,
                        ca.LicensePlate,
                        i.InvoiceDate,
                        i.RentalFee,
                        i.ExtraFee,
                        i.DepositDeduction,
                        i.TotalAmount,
                        i.PaymentStatus,
                        i.Notes
                    FROM Invoices i
                    INNER JOIN Contracts c ON i.ContractID = c.ContractID
                    INNER JOIN Customers cu ON c.CustomerID = cu.CustomerID
                    INNER JOIN Cars ca ON c.CarID = ca.CarID
                    ORDER BY i.InvoiceID DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Invoice invoice = new Invoice();

                            invoice.InvoiceID = Convert.ToInt32(reader["InvoiceID"]);
                            invoice.InvoiceNumber = reader["InvoiceNumber"].ToString();
                            invoice.ContractID = Convert.ToInt32(reader["ContractID"]);
                            invoice.ContractNumber = reader["ContractNumber"].ToString();
                            invoice.CustomerName = reader["CustomerName"].ToString();
                            invoice.LicensePlate = reader["LicensePlate"].ToString();
                            invoice.InvoiceDate = Convert.ToDateTime(reader["InvoiceDate"]);
                            invoice.RentalFee = Convert.ToDecimal(reader["RentalFee"]);
                            invoice.ExtraFee = Convert.ToDecimal(reader["ExtraFee"]);
                            invoice.DepositDeduction = Convert.ToDecimal(reader["DepositDeduction"]);

                            invoice.TotalAmount = reader["TotalAmount"] == DBNull.Value
                                ? (decimal?)null
                                : Convert.ToDecimal(reader["TotalAmount"]);

                            invoice.PaymentStatus = reader["PaymentStatus"].ToString();

                            invoice.Notes = reader["Notes"] == DBNull.Value
                                ? ""
                                : reader["Notes"].ToString();

                            invoices.Add(invoice);
                        }
                    }
                }
            }

            return invoices;
        }

        // Thêm hóa đơn
        public void Add(Invoice invoice)
        {
            using (SqlConnection conn = DbConnection.GetConnection())
            {
                string query = @"
                    INSERT INTO Invoices
                    (
                        InvoiceNumber,
                        ContractID,
                        InvoiceDate,
                        RentalFee,
                        ExtraFee,
                        DepositDeduction,
                        PaymentStatus,
                        Notes
                    )
                    VALUES
                    (
                        @InvoiceNumber,
                        @ContractID,
                        @InvoiceDate,
                        @RentalFee,
                        @ExtraFee,
                        @DepositDeduction,
                        @PaymentStatus,
                        @Notes
                    )";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@InvoiceNumber", SqlDbType.NVarChar, 60).Value =
                        invoice.InvoiceNumber;

                    cmd.Parameters.Add("@ContractID", SqlDbType.Int).Value =
                        invoice.ContractID;

                    cmd.Parameters.Add("@InvoiceDate", SqlDbType.DateTime).Value =
                        invoice.InvoiceDate;

                    SqlParameter rentalParam =
                        cmd.Parameters.Add("@RentalFee", SqlDbType.Decimal);
                    rentalParam.Precision = 18;
                    rentalParam.Scale = 2;
                    rentalParam.Value = invoice.RentalFee;

                    SqlParameter extraParam =
                        cmd.Parameters.Add("@ExtraFee", SqlDbType.Decimal);
                    extraParam.Precision = 18;
                    extraParam.Scale = 2;
                    extraParam.Value = invoice.ExtraFee;

                    SqlParameter depositParam =
                        cmd.Parameters.Add("@DepositDeduction", SqlDbType.Decimal);
                    depositParam.Precision = 18;
                    depositParam.Scale = 2;
                    depositParam.Value = invoice.DepositDeduction;

                    cmd.Parameters.Add("@PaymentStatus", SqlDbType.NVarChar, 30).Value =
                        invoice.PaymentStatus;

                    cmd.Parameters.Add("@Notes", SqlDbType.NVarChar, 1000).Value =
                        string.IsNullOrWhiteSpace(invoice.Notes)
                            ? (object)DBNull.Value
                            : invoice.Notes;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Cập nhật trạng thái thanh toán
        public void UpdatePaymentStatus(int invoiceID, string paymentStatus)
        {
            using (SqlConnection conn = DbConnection.GetConnection())
            {
                string query = @"
                    UPDATE Invoices
                    SET PaymentStatus = @PaymentStatus
                    WHERE InvoiceID = @InvoiceID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@PaymentStatus", SqlDbType.NVarChar, 30).Value =
                        paymentStatus;

                    cmd.Parameters.Add("@InvoiceID", SqlDbType.Int).Value =
                        invoiceID;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Xóa hóa đơn chưa thanh toán
        public bool Delete(int invoiceID)
        {
            using (SqlConnection conn = DbConnection.GetConnection())
            {
                string query = @"
                    DELETE FROM Invoices
                    WHERE InvoiceID = @InvoiceID
                    AND PaymentStatus = N'Chưa thanh toán'";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@InvoiceID", SqlDbType.Int).Value =
                        invoiceID;

                    conn.Open();

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
