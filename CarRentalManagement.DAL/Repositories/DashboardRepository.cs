
using System;
using System.Data.SqlClient;
using CarRentalManagement.Models.Entities;

namespace CarRentalManagement.DAL.Repositories
{
    public class DashboardRepository
    {
        public DashboardSummary GetSummary()
        {
            DashboardSummary summary = new DashboardSummary();

            string query = @"
                SELECT
                    (SELECT COUNT(*) FROM Cars)
                        AS TotalCars,

                    (SELECT COUNT(*)
                     FROM Cars
                     WHERE Status = N'Rented')
                        AS RentedCars,

                    (SELECT COUNT(*) FROM Customers)
                        AS TotalCustomers,

                    (SELECT COUNT(*)
                     FROM Contracts
                     WHERE Status = N'Active')
                        AS ActiveContracts,

                    (SELECT ISNULL(SUM(TotalAmount), 0)
                     FROM Invoices
                     WHERE PaymentStatus = N'Đã thanh toán')
                        AS TotalRevenue;
            ";

            using (SqlConnection conn = DbConnection.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            summary.TotalCars =
                                Convert.ToInt32(reader["TotalCars"]);

                            summary.RentedCars =
                                Convert.ToInt32(reader["RentedCars"]);

                            summary.TotalCustomers =
                                Convert.ToInt32(reader["TotalCustomers"]);

                            summary.ActiveContracts =
                                Convert.ToInt32(reader["ActiveContracts"]);

                            summary.TotalRevenue =
                                Convert.ToDecimal(reader["TotalRevenue"]);
                        }
                    }
                }
            }

            return summary;
        }
    }
}
