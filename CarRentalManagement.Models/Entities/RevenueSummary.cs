
using System.Collections.Generic;

namespace CarRentalManagement.Models.Entities
{
    public class RevenueSummary
    {
        public decimal TotalRevenue { get; set; }

        public int TotalInvoices { get; set; }

        public int PaidInvoices { get; set; }

        public int UnpaidInvoices { get; set; }

        public List<MonthlyRevenue> MonthlyRevenues { get; set; }

        public RevenueSummary()
        {
            MonthlyRevenues = new List<MonthlyRevenue>();
        }
    }

    public class MonthlyRevenue
    {
        public string Month { get; set; }

        public decimal Revenue { get; set; }
    }
}
