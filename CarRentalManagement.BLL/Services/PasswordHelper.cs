
using System;
using System.Security.Cryptography;

namespace CarRentalManagement.BLL.Services
{
    public static class PasswordHelper
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 100000;

        public static string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Mật khẩu không được để trống!");

            byte[] salt = new byte[SaltSize];

            using (var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(salt);
            }

            using (var pbkdf2 = new Rfc2898DeriveBytes(
                password, salt, Iterations))
            {
                byte[] hash = pbkdf2.GetBytes(HashSize);

                return Iterations + "." +
                       Convert.ToBase64String(salt) + "." +
                       Convert.ToBase64String(hash);
            }
        }

        public static bool VerifyPassword(
            string password, string storedHash)
        {
            if (string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(storedHash))
                return false;

            try
            {
                string[] parts = storedHash.Split('.');

                if (parts.Length != 3)
                    return false;

                int iterations = int.Parse(parts[0]);

                byte[] salt = Convert.FromBase64String(parts[1]);
                byte[] expectedHash =
                    Convert.FromBase64String(parts[2]);

                using (var pbkdf2 = new Rfc2898DeriveBytes(
                    password, salt, iterations))
                {
                    byte[] actualHash = pbkdf2.GetBytes(HashSize);

                    int difference = 0;

                    for (int i = 0; i < HashSize; i++)
                        difference |= actualHash[i] ^ expectedHash[i];

                    return difference == 0;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
