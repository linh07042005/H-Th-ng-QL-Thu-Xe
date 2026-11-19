
using System;
using CarRentalManagement.DAL.Repositories;
using CarRentalManagement.Models.Entities;

namespace CarRentalManagement.BLL.Services
{
    public class DashboardService
    {
        private readonly DashboardRepository dashboardRepository;

        public DashboardService()
        {
            dashboardRepository = new DashboardRepository();
        }

        public DashboardSummary GetSummary()
        {
            try
            {
                return dashboardRepository.GetSummary();
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Không thể tải dữ liệu Dashboard: " + ex.Message);
            }
        }
    }
}
