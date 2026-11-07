using CoffeSystem_DataAccess.DBContext;
using Microsoft.EntityFrameworkCore;
using CoffeSystem_DataAccess.Entities;

namespace CoffeSystem_DataAccess.Repositories
{

    public class DashboardRepository
    {
        private readonly AppDbContext _context;

         public  DashboardRepository(AppDbContext context)
        {
            _context = context;
        }

        // Methode N°1 : GetRevenue today
        public decimal GetTodayRevenue()
        {
            DateTime today = DateTime.Today;
            DateTime tomorrow = today.AddDays(1);

            return _context.Orders.Where(o => o.OrderDate >= today && o.OrderDate < tomorrow).Sum(o => o.TotalAmount);
        }

        // Methode N°2: GetOdersToday & GetOrdersTotal
        public int GetOrdersToday()
        {

            DateTime today = DateTime.Today;
            DateTime tomorrow = today.AddDays(1);
            return _context.Orders.Count(o => o.OrderDate >= today && o.OrderDate < tomorrow);


        }
        public int GetTotalOrders()
        {
            return _context.Orders.Count();
        }

        // Methode N°3: TotalProducts
        public int GetTotalProducts()
        {
            return _context.Products.Count(p => p.IsActive);
        }

        // Methode N°4 : Get Low Stock Product
        public async Task<int> GetLowStockAsync(int threshold)
        {
            return await _context.Products
                .CountAsync(p =>
                    p.IsActive &&
                    p.StockQuantity <= threshold);
        }


        // Methode N°5 : GetRecentOrders
        // Get Recent Orders
        public List<Order> GetRecentOrders(int count = 5)
        {
            return _context.Orders
                .AsNoTracking()
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderDate)
                .ThenByDescending(o => o.OrderId)
                .Take(count)
                .ToList();
        }

        // methode N°6 
        // Get Revenue for the Last 7 Days
        public async Task<List<(DateTime Date, decimal Revenue)>> GetLast7DaysRevenueAsync()
        {
            DateTime today = DateTime.Today;
            DateTime startDate = today.AddDays(-6);
            DateTime tomorrow = today.AddDays(1);

            var dailySales = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate < tomorrow)
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Revenue = g.Sum(o => o.TotalAmount)
                })
                .ToListAsync();

            return Enumerable.Range(0, 7)
                .Select(i =>
                {
                    DateTime date = startDate.AddDays(i);

                    return (
                        Date: date,
                        Revenue: dailySales
                            .Where(s => s.Date == date)
                            .Select(s => s.Revenue)
                            .FirstOrDefault()
                    );
                })
                .ToList();
        }
    }
}
