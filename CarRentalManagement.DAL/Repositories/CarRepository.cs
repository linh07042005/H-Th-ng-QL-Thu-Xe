
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using CarRentalManagement.Models.Entities;

namespace CarRentalManagement.DAL.Repositories
{
    public class CarRepository
    {
        // Lấy danh sách tất cả xe
        public List<Car> GetAll()
        {
            List<Car> cars = new List<Car>();

            string query = @"
                SELECT CarID, LicensePlate, Brand, Model,
                       ManufactureYear, CarType, DailyRate,
                       Status, Description, ImagePath
                FROM Cars";

            using (SqlConnection connection = DbConnection.GetConnection())
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Car car = new Car
                        {
                            CarID = Convert.ToInt32(reader["CarID"]),
                            LicensePlate = reader["LicensePlate"].ToString(),
                            Brand = reader["Brand"].ToString(),
                            Model = reader["Model"].ToString(),
                            ManufactureYear = Convert.ToInt32(reader["ManufactureYear"]),
                            CarType = reader["CarType"].ToString(),
                            DailyRate = Convert.ToDecimal(reader["DailyRate"]),
                            Status = reader["Status"].ToString(),

                            Description = reader["Description"] == DBNull.Value
                                ? ""
                                : reader["Description"].ToString(),

                            ImagePath = reader["ImagePath"] == DBNull.Value
                                ? ""
                                : reader["ImagePath"].ToString()
                        };

                        cars.Add(car);
                    }
                }
            }

            return cars;
        }

        // Thêm xe mới
        public bool Add(Car car)
        {
            string query = @"
                INSERT INTO Cars
                (
                    LicensePlate, Brand, Model, ManufactureYear,
                    CarType, DailyRate, Status, Description, ImagePath
                )
                VALUES
                (
                    @LicensePlate, @Brand, @Model, @ManufactureYear,
                    @CarType, @DailyRate, @Status, @Description, @ImagePath
                )";

            using (SqlConnection connection = DbConnection.GetConnection())
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@LicensePlate", car.LicensePlate);
                command.Parameters.AddWithValue("@Brand", car.Brand);
                command.Parameters.AddWithValue("@Model", car.Model);
                command.Parameters.AddWithValue("@ManufactureYear", car.ManufactureYear);
                command.Parameters.AddWithValue("@CarType", car.CarType);
                command.Parameters.AddWithValue("@DailyRate", car.DailyRate);
                command.Parameters.AddWithValue("@Status", car.Status);

                command.Parameters.AddWithValue("@Description",
                    string.IsNullOrWhiteSpace(car.Description)
                        ? (object)DBNull.Value
                        : car.Description);

                command.Parameters.AddWithValue("@ImagePath",
                    string.IsNullOrWhiteSpace(car.ImagePath)
                        ? (object)DBNull.Value
                        : car.ImagePath);

                connection.Open();

                return command.ExecuteNonQuery() > 0;
            }
        }

        // Cập nhật thông tin xe
        public bool Update(Car car)
        {
            string query = @"
                UPDATE Cars
                SET LicensePlate = @LicensePlate,
                    Brand = @Brand,
                    Model = @Model,
                    ManufactureYear = @ManufactureYear,
                    CarType = @CarType,
                    DailyRate = @DailyRate,
                    Status = @Status,
                    Description = @Description,
                    ImagePath = @ImagePath
                WHERE CarID = @CarID";

            using (SqlConnection connection = DbConnection.GetConnection())
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@CarID", car.CarID);
                command.Parameters.AddWithValue("@LicensePlate", car.LicensePlate);
                command.Parameters.AddWithValue("@Brand", car.Brand);
                command.Parameters.AddWithValue("@Model", car.Model);
                command.Parameters.AddWithValue("@ManufactureYear", car.ManufactureYear);
                command.Parameters.AddWithValue("@CarType", car.CarType);
                command.Parameters.AddWithValue("@DailyRate", car.DailyRate);
                command.Parameters.AddWithValue("@Status", car.Status);

                command.Parameters.AddWithValue("@Description",
                    string.IsNullOrWhiteSpace(car.Description)
                        ? (object)DBNull.Value
                        : car.Description);

                command.Parameters.AddWithValue("@ImagePath",
                    string.IsNullOrWhiteSpace(car.ImagePath)
                        ? (object)DBNull.Value
                        : car.ImagePath);

                connection.Open();

                return command.ExecuteNonQuery() > 0;
            }
        }

        // Xóa xe
        public bool Delete(int carID)
        {
            string query = "DELETE FROM Cars WHERE CarID = @CarID";

            using (SqlConnection connection = DbConnection.GetConnection())
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@CarID", carID);

                connection.Open();

                return command.ExecuteNonQuery() > 0;
            }
        }
    }
}
