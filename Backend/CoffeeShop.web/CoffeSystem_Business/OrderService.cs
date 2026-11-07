
using CoffeSystem_DataAccess.Entities;
using CoffeSystem_DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeSystem_Business
{
    public class OrderService
    {
        private readonly OrderRepository _repository;

        public OrderService(OrderRepository repository)
        {
            _repository = repository;
        }

        // Methode N°1 : Create Order
        public Task<int> CreateOrderAsync(Order order)
        {
            return _repository.CreateOrderAsync(order);
        }

        // Methode N°2 : Get All Orders
        public Task<List<Order>> GetAllOrdersAsync()
        {
            return _repository.GetAllOrdersAsync();
        }

        // Methode N°3 : Get Order By ID => Include User & OrderItems
        public Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return _repository.GetOrderByIdAsync(orderId);
        }

        // Methode N°4 : Get Revenue by day - last 7 days
        public Task<List<(DateTime Date, decimal Revenue)>>
            GetLast7DaysRevenueAsync()
        {
            return _repository.GetLast7DaysRevenueAsync();
        }

        // Methode N°5 : Get Today's Orders and Revenue by Hour
        public Task<List<(int Hour, int OrdersCount, decimal Revenue)>>
            GetTodayHourlyStatsAsync()
        {
            return _repository.GetTodayHourlyStatsAsync();
        }
    }
}
