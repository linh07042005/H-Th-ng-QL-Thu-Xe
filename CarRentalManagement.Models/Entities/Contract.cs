
using System;

namespace CarRentalManagement.Models.Entities
{
    public class Contract
    {
        public int ContractID { get; set; }
        public string ContractNumber { get; set; }
        public int CustomerID { get; set; }
        public int CarID { get; set; }
        public int UserID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpectedReturnDate { get; set; }
        public decimal DailyRate { get; set; }
        public decimal Deposit { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
    }
}
