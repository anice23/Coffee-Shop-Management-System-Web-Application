using CoffeSystem_DataAccess.DBContext;
using CoffeSystem_DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using CoffeSystem_DataAccess.Exceptions;
using System;
using System.Linq;


namespace CoffeSystem_DataAccess.Repositories
{
    public class OrderRepository
    {
        private readonly AppDbContext _context;
        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        // Methode N°1 : Create Order
        public async Task<int> CreateOrderAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (order.OrderItems == null || !order.OrderItems.Any())
            {
                throw new InvalidOrderException( "An order must contain at least one item." );
            }

            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction =
                    await _context.Database.BeginTransactionAsync(
                        System.Data.IsolationLevel.Serializable
                    );

                try
                {
                    var items = order.OrderItems.ToList();

                    if (items.Any(item => item == null))
                    {
                        throw new InvalidOperationException("The order contains an invalid item.");
                    }

                    // Validate quantities
                    if (items.Any(item => item.Quantity <= 0))
                    {
                        throw new InvalidOrderException(  "Product quantity must be greater than zero.");
                    }

                    // Group quantities by product
                    var requestedQuantities = items
                        .GroupBy(item => item.ProductId)
                        .ToDictionary(
                            group => group.Key,
                            group => group.Sum(item => item.Quantity)
                        );

                    var productsById = new Dictionary<int, Product>();

                    // Check product existence and available stock
                    foreach (var request in requestedQuantities)
                    {
                        var product = await _context.Products
                            .FirstOrDefaultAsync(
                                p => p.ProductId == request.Key
                            );

                        if (product == null)
                        {
                            throw new ProductNotFoundException($"Product ID {request.Key} was not found.");
                        }

                        if (product.StockQuantity < request.Value)
                        {
                            throw new InsufficientStockException(
                              $"Not enough stock for {product.Name}. " +
                              $"Available: {product.StockQuantity}, " +
                              $"Requested: {request.Value}."
                             );
                        }

                        productsById.Add(product.ProductId, product);
                    }

                    // Use prices from the database, not the UI cart
                    foreach (var item in items)
                    {
                        var product = productsById[item.ProductId];

                        item.UnitPrice = product.Price;
                        item.TotalPrice = item.UnitPrice * item.Quantity;
                    }

                    // Update stock after all checks succeed
                    foreach (var request in requestedQuantities)
                    {
                        productsById[request.Key].StockQuantity -=
                            request.Value;
                    }

                    // Calculate total on the backend
                    order.TotalAmount = items.Sum(item => item.TotalPrice);

                    _context.Orders.Add(order);

                    await _context.SaveChangesAsync();

                    int orderId = order.OrderId;

                    await transaction.CommitAsync();

                    return orderId;
                }
                catch
                {
                    try
                    {
                        await transaction.RollbackAsync();
                    }
                    catch
                    {
                        // Preserve the original exception.
                    }

                    _context.ChangeTracker.Clear();

                    throw;
                }
            });
        }


        // Methode N°2 : Get All Orders
        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .AsNoTracking()
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.OrderDate)
                .ThenByDescending(o => o.OrderId)
                .ToListAsync();
        }

        // Methode N°3 : Get Orders By ID => Include User and OrderItems
        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }


        // Methode N°4 : Get Revenues by day last 7 days
        public async Task<List<(DateTime Date, decimal Revenue)>> GetLast7DaysRevenueAsync()
        {
            DateTime startDate = DateTime.Today.AddDays(-6);
            DateTime endDate = DateTime.Today.AddDays(1);

            var data = await _context.Orders
                .AsNoTracking()
                .Where(o => o.OrderDate >= startDate &&
                            o.OrderDate < endDate)
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Revenue = g.Sum(o => o.TotalAmount)
                })
                .ToListAsync();

            List<(DateTime Date, decimal Revenue)> result = new();

            for (int i = 0; i < 7; i++)
            {
                DateTime date = startDate.AddDays(i);

                var day = data.FirstOrDefault(x => x.Date == date);

                result.Add((
                    date,
                    day?.Revenue ?? 0
                ));
            }

            return result;
        }

        // Methode N°5 : Get Today's Orders and Revenue by Hour
        public async Task<List<(int Hour, int OrdersCount, decimal Revenue)>>  GetTodayHourlyStatsAsync()
        {
            DateTime startDate = DateTime.Today;
            DateTime endDate = startDate.AddDays(1);

            var data = await _context.Orders
                .AsNoTracking()
                .Where(o => o.OrderDate >= startDate &&
                            o.OrderDate < endDate)
                .GroupBy(o => o.OrderDate.Hour)
                .Select(g => new
                {
                    Hour = g.Key,
                    OrdersCount = g.Count(),
                    Revenue = g.Sum(o => o.TotalAmount)
                })
                .ToListAsync();

            List<(int Hour, int OrdersCount, decimal Revenue)> result = new();

            for (int hour = 0; hour < 24; hour++)
            {
                var currentHour = data.FirstOrDefault(x => x.Hour == hour);

                result.Add((
                    hour,
                    currentHour?.OrdersCount ?? 0,
                    currentHour?.Revenue ?? 0
                ));
            }

            return result;
        }

    }
}
