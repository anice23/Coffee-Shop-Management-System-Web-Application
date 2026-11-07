
namespace CoffeSystem_DataAccess.DTOs
{
    public class DashboardDataDto
    {
        public decimal TodayRevenue { get; set; }

        public int TotalOrders { get; set; }
        public int TodayOrders { get; set; }

        public int TotalProducts { get; set; }

        public int LowStockProducts { get; set; }

        public List<DailyRevenueDataDto> WeeklySales { get; set; } = new();

        public List<RecentOrderDataDto> RecentOrders { get; set; } = new();
    }

    public class DailyRevenueDataDto
    {
        public DateTime Date { get; set; }

        public decimal Revenue { get; set; }
    }

    public class RecentOrderDataDto
    {
        public int OrderId { get; set; }

        public string CashierName { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;

        public DateTime OrderDate { get; set; }
    }
}
