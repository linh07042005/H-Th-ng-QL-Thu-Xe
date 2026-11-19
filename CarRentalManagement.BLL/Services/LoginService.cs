
using System;
using CarRentalManagement.DAL.Repositories;
using CarRentalManagement.Models.Entities;

namespace CarRentalManagement.BLL.Services
{
    public class LoginService
    {
        private readonly UserRepository userRepository;

        public LoginService()
        {
            userRepository = new UserRepository();
        }

        public User Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new Exception("Vui lòng nhập tên đăng nhập!");

            if (string.IsNullOrWhiteSpace(password))
                throw new Exception("Vui lòng nhập mật khẩu!");

            User user = userRepository.GetByUsername(username.Trim());

            if (user == null)
                throw new Exception("Tên đăng nhập hoặc mật khẩu không chính xác!");

            if (!user.IsActive)
                throw new Exception("Tài khoản đã bị khóa!");

            if (!PasswordHelper.VerifyPassword(
                password, user.PasswordHash))
            {
                throw new Exception(
                    "Tên đăng nhập hoặc mật khẩu không chính xác!");
            }

            return user;
        }
    }
}
