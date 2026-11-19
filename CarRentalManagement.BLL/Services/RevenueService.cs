
using System;
using CarRentalManagement.DAL.Repositories;
using CarRentalManagement.Models.Entities;

namespace CarRentalManagement.BLL.Services
{
    public class RevenueService
    {
        private readonly RevenueRepository revenueRepository;

        public RevenueService()
        {
            revenueRepository = new RevenueRepository();
        }

        public RevenueSummary GetRevenueSummary(int year)
        {
            try
            {
                if (year < 2000 || year > 2100)
                    throw new Exception("Năm thống kê không hợp lệ!");

                return revenueRepository.GetRevenueSummary(year);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Không thể tải dữ liệu thống kê doanh thu: "
                    + ex.Message);
            }
        }
    }
}
