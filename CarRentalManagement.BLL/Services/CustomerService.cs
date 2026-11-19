
using System;
using System.Collections.Generic;
using CarRentalManagement.DAL.Repositories;
using CarRentalManagement.Models.Entities;

namespace CarRentalManagement.BLL.Services
{
    public class CustomerService
    {
        private readonly CustomerRepository customerRepository;

        public CustomerService()
        {
            customerRepository = new CustomerRepository();
        }

        // Lấy danh sách khách hàng
        public List<Customer> GetAllCustomers()
        {
            return customerRepository.GetAll();
        }

        // Thêm khách hàng
        public bool AddCustomer(Customer customer)
        {
            ValidateCustomer(customer);
            return customerRepository.Add(customer);
        }

        // Cập nhật khách hàng
        public bool UpdateCustomer(Customer customer)
        {
            if (customer == null || customer.CustomerID <= 0)
                throw new Exception("Mã khách hàng không hợp lệ!");

            ValidateCustomer(customer);
            return customerRepository.Update(customer);
        }

        // Xóa khách hàng
        public bool DeleteCustomer(int customerID)
        {
            if (customerID <= 0)
                throw new Exception("Mã khách hàng không hợp lệ!");

            return customerRepository.Delete(customerID);
        }

        // Kiểm tra thông tin khách hàng
        private void ValidateCustomer(Customer customer)
        {
            if (customer == null)
                throw new Exception("Thông tin khách hàng không được để trống!");

            if (string.IsNullOrWhiteSpace(customer.FullName))
                throw new Exception("Vui lòng nhập họ tên khách hàng!");

            if (string.IsNullOrWhiteSpace(customer.Phone))
                throw new Exception("Vui lòng nhập số điện thoại!");

            if (string.IsNullOrWhiteSpace(customer.CitizenID))
                throw new Exception("Vui lòng nhập số CCCD!");

            if (customer.Phone.Length < 10 || customer.Phone.Length > 11)
                throw new Exception("Số điện thoại không hợp lệ!");

            if (!string.IsNullOrWhiteSpace(customer.Email) &&
                !customer.Email.Contains("@"))
                throw new Exception("Email không hợp lệ!");
        }
    }
}
