
using CoffeSystem_Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeSystem_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Cashier")]
    public class ReportsController : ControllerBase
    {
        private readonly ReportService _reportService;

        public ReportsController(ReportService reportService)
        {
            _reportService = reportService;
        }

        // Endpoint N°1: Get Sales Report
        [HttpGet("sales")]
        public async Task<IActionResult> GetSalesReport([FromQuery] DateTime fromDate,[FromQuery] DateTime toDate)
        {
            try
            {
                var report = await _reportService.GetSalesReportAsync(
                    fromDate,
                    toDate);

                return Ok(report);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Endpoint N°2: Get Sales Summary
        [HttpGet("summary")]
        public async Task<IActionResult> GetSalesSummary( [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            try
            {
                var summary = await _reportService.GetSalesSummaryAsync(
                    fromDate,
                    toDate);

                return Ok(summary);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Endpoint N°3: Get Best Selling Products
        [HttpGet("best-selling-products")]
        public async Task<IActionResult> GetBestSellingProducts( [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            try
            {
                var products = await _reportService.GetBestSellingProductsAsync(
                    fromDate,
                    toDate);

                return Ok(products);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        // Endpoint N°4 : Get Daily Sales Reports
        [HttpGet("daily-sales")]
        public async Task<IActionResult> GetDailySalesReport( [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            try
            {
                var report = await _reportService.GetDailySalesReportAsync(
                    fromDate,
                    toDate);

                return Ok(report);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
