using System.Data.SqlClient;

namespace CarRentalManagement.DAL
{
    public class DbConnection
    {
        private static readonly string connectionString =
            @"Data Source=LAPTOP-TIAT3U08\SQLEXPRESS;Initial Catalog=LINHXURY;Integrated Security=True";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}