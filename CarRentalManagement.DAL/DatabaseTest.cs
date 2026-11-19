using System;

namespace CarRentalManagement.DAL
{
    public class DatabaseTest
    {
        public static bool TestConnection()
        {
            try
            {
                using (var connection = DbConnection.GetConnection())
                {
                    connection.Open();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}