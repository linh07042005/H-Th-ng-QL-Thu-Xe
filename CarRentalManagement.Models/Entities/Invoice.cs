
using System;

namespace CarRentalManagement.Models.Entities
{
    public class Invoice
    {
        public int InvoiceID { get; set; }

        public string InvoiceNumber { get; set; }

        public int ContractID { get; set; }

        public string ContractNumber { get; set; }

        public string CustomerName { get; set; }

        public string LicensePlate { get; set; }

        public DateTime InvoiceDate { get; set; }

        public decimal RentalFee { get; set; }

        public decimal ExtraFee { get; set; }

        public decimal DepositDeduction { get; set; }

        public decimal? TotalAmount { get; set; }

        public string PaymentStatus { get; set; }

        public string Notes { get; set; }
    }
}
