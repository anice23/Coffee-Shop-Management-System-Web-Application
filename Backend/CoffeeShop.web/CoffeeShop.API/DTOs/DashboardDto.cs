namespace CoffeSystem_API.DTOs
{
    public class DashboardDto
    {
        public decimal TodayRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int TodayOrders { get; set; }

        public int TotalProducts { get; set; }
        public int LowStockProducts { get; set; }

        public List<DailyRevenueDto> WeeklySales { get; set; } = new();

        public List<RecentOrderDto> RecentOrders { get; set; } = new();
    }

    public class DailyRevenueDto
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
    }

    public class RecentOrderDto
    {
        public int OrderId { get; set; }
        public string CashierName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
    }
}