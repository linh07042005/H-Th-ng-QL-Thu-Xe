
using System;
using System.Collections.Generic;
using CarRentalManagement.DAL.Repositories;
using CarRentalManagement.Models.Entities;

namespace CarRentalManagement.BLL.Services
{
    public class ContractService
    {
        private readonly ContractRepository contractRepository;

        public ContractService()
        {
            contractRepository = new ContractRepository();
        }

        // Lấy danh sách hợp đồng
        public List<Contract> GetAllContracts()
        {
            return contractRepository.GetAll();
        }

        // Thêm hợp đồng
        public bool AddContract(Contract contract)
        {
            ValidateContract(contract);

            return contractRepository.Add(contract);
        }

        // Cập nhật hợp đồng
        public bool UpdateContract(Contract contract)
        {
            if (contract == null || contract.ContractID <= 0)
                throw new Exception("Mã hợp đồng không hợp lệ!");

            ValidateContract(contract);

            return contractRepository.Update(contract);
        }

        // Xóa hợp đồng
        public bool DeleteContract(int contractID)
        {
            if (contractID <= 0)
                throw new Exception("Mã hợp đồng không hợp lệ!");

            return contractRepository.Delete(contractID);
        }

        // Kiểm tra thông tin hợp đồng
        private void ValidateContract(Contract contract)
        {
            if (contract == null)
                throw new Exception("Thông tin hợp đồng không được để trống!");

            if (string.IsNullOrWhiteSpace(contract.ContractNumber))
                throw new Exception("Vui lòng nhập mã hợp đồng!");

            if (contract.CustomerID <= 0)
                throw new Exception("Vui lòng chọn khách hàng!");

            if (contract.CarID <= 0)
                throw new Exception("Vui lòng chọn xe!");

            if (contract.UserID <= 0)
                throw new Exception("Vui lòng chọn nhân viên!");

            if (contract.StartDate >= contract.ExpectedReturnDate)
                throw new Exception("Ngày trả xe phải sau ngày thuê!");

            if (contract.DailyRate <= 0)
                throw new Exception("Giá thuê phải lớn hơn 0!");

            if (contract.Deposit < 0)
                throw new Exception("Tiền đặt cọc không được âm!");

            if (string.IsNullOrWhiteSpace(contract.Status))
                throw new Exception("Vui lòng chọn trạng thái hợp đồng!");
        }
    }
}
