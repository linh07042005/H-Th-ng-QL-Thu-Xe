using System;

namespace CarRentalManagement.Models.Entities
{
    public class VehicleHandover
    {
        public int HandoverID { get; set; }

        public int ContractID { get; set; }

        public string ContractNumber { get; set; }

        public string CustomerName { get; set; }

        public string LicensePlate { get; set; }

        public string HandoverType { get; set; }

        public DateTime HandoverDate { get; set; }

        public int Odometer { get; set; }

        public string FuelLevel { get; set; }

        public string VehicleCondition { get; set; }

        public string Notes { get; set; }

        public int UserID { get; set; }

        public string EmployeeName { get; set; }
    }
}