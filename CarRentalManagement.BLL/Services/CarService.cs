
using System;
using System.Collections.Generic;
using CarRentalManagement.DAL.Repositories;
using CarRentalManagement.Models.Entities;

namespace CarRentalManagement.BLL.Services
{
    public class CarService
    {
        private readonly CarRepository carRepository;

        public CarService()
        {
            carRepository = new CarRepository();
        }

        // Lấy danh sách xe
        public List<Car> GetAllCars()
        {
            return carRepository.GetAll();
        }

        // Thêm xe
        public bool AddCar(Car car)
        {
            ValidateCar(car);
            return carRepository.Add(car);
        }

        // Cập nhật xe
        public bool UpdateCar(Car car)
        {
            if (car.CarID <= 0)
                throw new Exception("Mã xe không hợp lệ!");

            ValidateCar(car);
            return carRepository.Update(car);
        }

        // Xóa xe
        public bool DeleteCar(int carID)
        {
            if (carID <= 0)
                throw new Exception("Mã xe không hợp lệ!");

            return carRepository.Delete(carID);
        }

        // Kiểm tra thông tin xe
        private void ValidateCar(Car car)
        {
            if (car == null)
                throw new Exception("Thông tin xe không được để trống!");

            if (string.IsNullOrWhiteSpace(car.LicensePlate))
                throw new Exception("Vui lòng nhập biển số xe!");

            if (string.IsNullOrWhiteSpace(car.Brand))
                throw new Exception("Vui lòng nhập hãng xe!");

            if (string.IsNullOrWhiteSpace(car.Model))
                throw new Exception("Vui lòng nhập mẫu xe!");

            if (car.ManufactureYear < 1980 || car.ManufactureYear > DateTime.Now.Year + 1)
                throw new Exception("Năm sản xuất không hợp lệ!");

            if (car.DailyRate <= 0)
                throw new Exception("Giá thuê xe phải lớn hơn 0!");

            if (string.IsNullOrWhiteSpace(car.Status))
                throw new Exception("Vui lòng chọn trạng thái xe!");
        }
    }
}
