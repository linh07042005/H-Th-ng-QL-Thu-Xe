
using System;
using System.Collections.Generic;
using CarRentalManagement.DAL.Repositories;
using CarRentalManagement.Models.Entities;

namespace CarRentalManagement.BLL.Services
{
    public class VehicleHandoverService
    {
        private readonly VehicleHandoverRepository handoverRepository;

        public VehicleHandoverService()
        {
            handoverRepository = new VehicleHandoverRepository();
        }

        // Lấy danh sách biên bản giao nhận
        public List<VehicleHandover> GetAllHandovers()
        {
            return handoverRepository.GetAll();
        }

        // Ghi nhận giao xe hoặc nhận xe
        public bool RegisterHandover(VehicleHandover handover)
        {
            ValidateHandover(handover);

            if (handover.HandoverType != "Giao xe" &&
                handover.HandoverType != "Nhận xe")
            {
                throw new Exception("Loại bàn giao không hợp lệ!");
            }

            handoverRepository.Add(handover);

            return true;
        }

        // Kiểm tra thông tin biên bản
        private void ValidateHandover(VehicleHandover handover)
        {
            if (handover == null)
                throw new Exception("Thông tin bàn giao không được để trống!");

            if (handover.ContractID <= 0)
                throw new Exception("Vui lòng chọn hợp đồng!");

            if (handover.UserID <= 0)
                throw new Exception("Không xác định được nhân viên thực hiện!");

            if (handover.HandoverDate > DateTime.Now)
                throw new Exception("Ngày bàn giao không được ở tương lai!");

            if (handover.Odometer < 0)
                throw new Exception("Số km không hợp lệ!");

            int fuel;

            if (!int.TryParse(handover.FuelLevel, out fuel))
                throw new Exception("Vui lòng nhập mức nhiên liệu hợp lệ!");

            if (fuel < 0 || fuel > 100)
                throw new Exception("Mức nhiên liệu phải từ 0 đến 100!");

            if (string.IsNullOrWhiteSpace(handover.VehicleCondition))
                throw new Exception("Vui lòng nhập tình trạng xe!");
        }
    }
}
