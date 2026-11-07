using CoffeSystem_Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CoffeSystem_API.DTOs;


namespace CoffeSystem_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Cashier")]
      public class DashboardController : ControllerBase
    {
        private readonly DashboardService _dashboardService;

        public DashboardController(DashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<ActionResult<DashboardDto>> GetDashboard()
        {
            var weeklySales = await _dashboardService.GetLast7DaysRevenueAsync();

            var dashboard = new DashboardDto
            {
                TodayRevenue = _dashboardService.GetTodayRevenue(),
                TotalOrders = _dashboardService.GetTotalOrders(),
                TotalProducts = _dashboardService.GetTotalProducts(),
                LowStockProducts = await _dashboardService.GetLowStockAsync(),
                TodayOrders = _dashboardService.GetTodayOrders(),

                WeeklySales = weeklySales.Select(s => new DailyRevenueDto
                {
                    Date = s.Date,
                    Revenue = s.Revenue
                }).ToList(),

                RecentOrders = _dashboardService.GetRecentOrders().Select(o => new RecentOrderDto
                {
                    OrderId = o.OrderId,
                    CashierName = o.User.FullName,
                    TotalAmount = o.TotalAmount,
                    PaymentMethod = o.PaymentMethod,
                    OrderDate = o.OrderDate
                }).ToList()
            };

            return Ok(dashboard);
        }
    }
}