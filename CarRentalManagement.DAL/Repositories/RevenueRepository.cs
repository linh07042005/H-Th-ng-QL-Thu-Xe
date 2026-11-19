
using System;
using System.Data;
using System.Data.SqlClient;
using CarRentalManagement.Models.Entities;

namespace CarRentalManagement.DAL.Repositories
{
    public class RevenueRepository
    {
        public RevenueSummary GetRevenueSummary(int year)
        {
            RevenueSummary summary = new RevenueSummary();

            DateTime startDate = new DateTime(year, 1, 1);
            DateTime endDate = new DateTime(year + 1, 1, 1);

            using (SqlConnection conn = DbConnection.GetConnection())
            {
                conn.Open();

                // 1. Thống kê tổng quan hóa đơn theo năm
                string summaryQuery = @"
                    SELECT
                        ISNULL(SUM(
                            CASE
                                WHEN PaymentStatus = N'Đã thanh toán'
                                THEN TotalAmount
                                ELSE 0
                            END
                        ), 0) AS TotalRevenue,

                        COUNT(*) AS TotalInvoices,

                        ISNULL(SUM(
                            CASE
                                WHEN PaymentStatus = N'Đã thanh toán'
                                THEN 1 ELSE 0
                            END
                        ), 0) AS PaidInvoices,

                        ISNULL(SUM(
                            CASE
                                WHEN PaymentStatus = N'Chưa thanh toán'
                                THEN 1 ELSE 0
                            END
                        ), 0) AS UnpaidInvoices

                    FROM Invoices
                    WHERE InvoiceDate >= @StartDate
                      AND InvoiceDate < @EndDate";

                using (SqlCommand cmd = new SqlCommand(summaryQuery, conn))
                {
                    cmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = startDate;
                    cmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = endDate;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            summary.TotalRevenue =
                                reader["TotalRevenue"] == DBNull.Value
                                ? 0
                                : Convert.ToDecimal(reader["TotalRevenue"]);

                            summary.TotalInvoices =
                                reader["TotalInvoices"] == DBNull.Value
                                ? 0
                                : Convert.ToInt32(reader["TotalInvoices"]);

                            summary.PaidInvoices =
                                reader["PaidInvoices"] == DBNull.Value
                                ? 0
                                : Convert.ToInt32(reader["PaidInvoices"]);

                            summary.UnpaidInvoices =
                                reader["UnpaidInvoices"] == DBNull.Value
                                ? 0
                                : Convert.ToInt32(reader["UnpaidInvoices"]);
                        }
                    }
                }

                // 2. Thống kê doanh thu từng tháng
                string monthlyQuery = @"
                    SELECT
                        MONTH(InvoiceDate) AS RevenueMonth,
                        ISNULL(SUM(TotalAmount), 0) AS Revenue
                    FROM Invoices
                    WHERE PaymentStatus = N'Đã thanh toán'
                      AND InvoiceDate >= @StartDate
                      AND InvoiceDate < @EndDate
                    GROUP BY MONTH(InvoiceDate)
                    ORDER BY RevenueMonth";

                using (SqlCommand cmd = new SqlCommand(monthlyQuery, conn))
                {
                    cmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = startDate;
                    cmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = endDate;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MonthlyRevenue monthly = new MonthlyRevenue
                            {
                                Month = "Tháng " + reader["RevenueMonth"].ToString(),

                                Revenue = reader["Revenue"] == DBNull.Value
                                    ? 0
                                    : Convert.ToDecimal(reader["Revenue"])
                            };

                            summary.MonthlyRevenues.Add(monthly);
                        }
                    }
                }
            }

            return summary;
        }
    }
}
