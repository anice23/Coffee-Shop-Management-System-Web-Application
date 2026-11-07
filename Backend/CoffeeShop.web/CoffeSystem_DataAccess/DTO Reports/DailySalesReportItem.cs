namespace CoffeSystem_DataAccess.DTO_Reports
{
    public class DailySalesReportItem
    {
        public string Date { get; set; } = string.Empty;

        public int OrdersCount { get; set; }

        public decimal Revenue { get; set; }
    }
}