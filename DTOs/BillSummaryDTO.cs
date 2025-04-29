namespace MR_power.DTOs
{
    public class BillSummaryDTO
    {
        public int TotalBills { get; set; }
        public decimal TotalSales { get; set; }
        public decimal AverageBillAmount { get; set; }
        public int PendingBills { get; set; }
        public int PaidBills { get; set; }
    }
} 