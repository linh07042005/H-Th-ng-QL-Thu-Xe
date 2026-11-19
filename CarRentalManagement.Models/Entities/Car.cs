
namespace CarRentalManagement.Models.Entities
{
    public class Car
    {
        public int CarID { get; set; }

        public string LicensePlate { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        public int ManufactureYear { get; set; }

        public string CarType { get; set; }

        public decimal DailyRate { get; set; }

        public string Status { get; set; }

        public string Description { get; set; }

        // Đường dẫn hình ảnh xe
        public string ImagePath { get; set; }
    }
}
