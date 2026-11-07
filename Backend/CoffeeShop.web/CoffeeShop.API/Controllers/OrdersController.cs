using CoffeSystem_API.DTOs;
using CoffeSystem_Business;
using CoffeSystem_DataAccess.Entities;
using CoffeSystem_DataAccess.Exceptions;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace CoffeSystem_API.Controllers
{
    public class CreateOrderRequest
    {
        public string PaymentMethod { get; set; } = "Cash";

        public List<CreateOrderItemRequest> Items { get; set; } = new();
    }

    public class CreateOrderItemRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Cashier")]

    public class OrdersController : ControllerBase
    {
        private readonly OrderService _orderService;
        private readonly IAntiforgery _antiforgery;

        public OrdersController(
            OrderService orderService,
            IAntiforgery antiforgery)
        {
            _orderService = orderService;
            _antiforgery = antiforgery;
        }

        // Endpoint N°1 : Create Order
        [HttpPost]
        public async Task<IActionResult> CreateOrder( [FromBody] CreateOrderRequest request)
        {
            // 1. Validate CSRF
            try
            {
                await _antiforgery.ValidateRequestAsync(HttpContext);
            }
            catch (AntiforgeryValidationException)
            {
                return BadRequest(new
                {
                    message = "CSRF validation failed."
                });
            }

            // 2. Validate request
            if (request.Items == null || !request.Items.Any())
            {
                return BadRequest(new
                {
                    message = "Order must contain at least one item."
                });
            }

            if (request.PaymentMethod != "Cash" &&
                request.PaymentMethod != "Card")
            {
                return BadRequest(new
                {
                    message = "Invalid payment method."
                });
            }

            // 3. Get authenticated UserId from JWT Claims
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new
                {
                    message = "Invalid authenticated user."
                });
            }

            // 4. Create Order Entity
            var order = new Order
            {
                UserId = userId,
                PaymentMethod = request.PaymentMethod,
                OrderItems = request.Items.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                }).ToList()
            };

            // 5. Save Order through Business Layer
            try
            {
                var orderId = await _orderService.CreateOrderAsync(order);

                return Ok(new
                {
                    orderId,
                    message = "Order created successfully."
                });
            }
            catch (ProductNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InsufficientStockException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (InvalidOrderException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        // Endpoint N°2 : Get All Orders
        [HttpGet]
        public async Task<ActionResult<List<OrderDto>>> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();

            var result = orders.Select(o => new OrderDto
            {
                OrderId = o.OrderId,
                CashierName = o.User?.FullName ?? "Unknown",
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                PaymentMethod = o.PaymentMethod,

                Items = o.OrderItems.Select(item => new OrderItemDto
                {
                    ProductName = item.Product?.Name ?? "Unknown",
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.TotalPrice
                }).ToList()
            }).ToList();

            return Ok(result);
        }


        // Endpoint N°3 : Get Today's Orders and Revenue by Hour
        [HttpGet("today-stats")]
        public async Task<IActionResult> GetTodayStats()
        {
            var stats = await _orderService.GetTodayHourlyStatsAsync();

            var result = stats.Select(s => new
            {
                hour = s.Hour,
                ordersCount = s.OrdersCount,
                revenue = s.Revenue
            }).ToList();

            return Ok(result);
        }
    }






}