
using CoffeSystem_DataAccess.DBContext;
using CoffeSystem_DataAccess.DTO_Reports;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace CoffeSystem_DataAccess.Repositories
{
    public class ReportRepository
    {
        // 
        private readonly AppDbContext _context;

        public ReportRepository(AppDbContext context)
        {
            _context = context;
        }

        // Methode N°1 => Get all orders dto 
        public async Task<List<SalesReportItem>> GetSalesReportAsync(DateTime fromDate, DateTime toDate)
        {
            DateTime endDateExeclusive = toDate.AddDays(1);

            return await _context.Orders
                .AsNoTracking()
                .Where(o => o.OrderDate >= fromDate && o.OrderDate < endDateExeclusive).
                OrderByDescending(o => o.OrderDate).
                Select(o => new SalesReportItem
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate,
                    PaymentMethod = o.PaymentMethod,
                    TotalAmount = o.TotalAmount
                }).
                ToListAsync();
        }


        // Methode N°2 : Salles Summary
        public async Task<SalesSummaryReport> GetSalesSummaryAsync(DateTime fromDate, DateTime toDate)
        {
            DateTime startDate = fromDate.Date;
            DateTime endDateExclusive = toDate.Date.AddDays(1);

            var ordersQuery = _context.Orders
                .AsNoTracking()
                .Where(o =>
                    o.OrderDate >= startDate &&
                    o.OrderDate < endDateExclusive);

            int totalOrders = await ordersQuery.CountAsync();

            decimal totalRevenue = await ordersQuery
                .SumAsync(o => (decimal?)o.TotalAmount) ?? 0m;

            decimal averageOrderValue = totalOrders > 0
                ? totalRevenue / totalOrders
                : 0m;

            var bestSellingProduct = await _context.OrderItems
                .AsNoTracking()
                .Where(oi =>
                    oi.Order.OrderDate >= startDate &&
                    oi.Order.OrderDate < endDateExclusive)
                .GroupBy(oi => new
                {
                    oi.ProductId,
                    oi.Product.Name
                })
                .Select(g => new
                {
                    ProductName = g.Key.Name,
                    TotalQuantity = g.Sum(oi => oi.Quantity)
                })
                .OrderByDescending(x => x.TotalQuantity)
                .ThenBy(x => x.ProductName)
                .FirstOrDefaultAsync();

            return new SalesSummaryReport
            {
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                AverageOrderValue = averageOrderValue,
                BestSellingProduct = bestSellingProduct?.ProductName ?? "N/A",
                BestSellingProductQuantity =
                    bestSellingProduct?.TotalQuantity ?? 0
            };
        }

        // Methode N°3 : Most Salles Product

        public async Task<List<ProductSalesReportItem>>
            GetBestSellingProductsAsync(
                DateTime fromDate,
                DateTime toDate)
        {
            DateTime startDate = fromDate.Date;
            DateTime endDateExclusive = toDate.Date.AddDays(1);

            var products = await _context.OrderItems
                .AsNoTracking()
                .Where(oi =>
                    oi.Order.OrderDate >= startDate &&
                    oi.Order.OrderDate < endDateExclusive)
                .GroupBy(oi => new
                {
                    oi.ProductId,
                    oi.Product.Name
                })
                .Select(g => new ProductSalesReportItem
                {
                    ProductName = g.Key.Name,
                    TotalQuantity = g.Sum(oi => oi.Quantity),
                    TotalRevenue = g.Sum(oi => oi.TotalPrice)
                })
                .OrderByDescending(p => p.TotalQuantity)
                .ThenByDescending(p => p.TotalRevenue)
                .Take(5)
                .ToListAsync();

            return products;
        }

    

    // 
    public async Task<List<DailySalesReportItem>> GetDailySalesReportAsync(DateTime fromDate, DateTime toDate)
        {
            DateTime startDate = fromDate.Date;
            DateTime endDate = toDate.Date;

            DateTime endDateExclusive = endDate.AddDays(1);

            // 1. Get daily sales from database
            var dailySales = await _context.Orders
                .AsNoTracking()
                .Where(o => o.OrderDate >= startDate &&
                            o.OrderDate < endDateExclusive)
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    OrdersCount = g.Count(),
                    Revenue = g.Sum(o => o.TotalAmount)
                })
                .ToListAsync();

            // 2. Convert results into a dictionary
            var salesByDate = dailySales.ToDictionary(
                x => x.Date.Date
            );

            // 3. Build a report containing every day
            var reports = new List<DailySalesReportItem>();

            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                salesByDate.TryGetValue(date, out var dailySale);

                reports.Add(new DailySalesReportItem
                {
                    Date = date.ToString("yyyy-MM-dd"),
                    OrdersCount = dailySale?.OrdersCount ?? 0,
                    Revenue = dailySale?.Revenue ?? 0m
                });
            }

            return reports;
        }
    }

    

}