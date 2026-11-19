
namespace CarRentalManagement.Models.Entities
{
    public class DashboardSummary
    {
        // Tổng số xe
        public int TotalCars { get; set; }

        // Số xe đang được thuê
        public int RentedCars { get; set; }

        // Tổng số khách hàng
        public int TotalCustomers { get; set; }

        // Số hợp đồng đang hoạt động
        public int ActiveContracts { get; set; }

        // Tổng doanh thu đã thanh toán
        public decimal TotalRevenue { get; set; }
    }
}
