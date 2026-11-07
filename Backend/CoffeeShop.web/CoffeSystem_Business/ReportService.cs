
using CoffeSystem_DataAccess.DTO_Reports;
using CoffeSystem_DataAccess.DTO_Reports;
using CoffeSystem_DataAccess.Repositories;

namespace CoffeSystem_Business
{
    public class ReportService
    {
        private readonly ReportRepository _repository;

        public ReportService(ReportRepository repository)
        {
            _repository = repository;
        }

        // Methode N°1
        public async Task<List<SalesReportItem>> GetSalesReportAsync(DateTime fromDate,DateTime toDate)
        {
            if (fromDate.Date > toDate.Date)
            {
                throw new ArgumentException(
                    "The start date cannot be after the end date.");
            }

            return await _repository.GetSalesReportAsync(
                fromDate.Date,
                toDate.Date);
        }

        // Methode N°2

        public async Task<SalesSummaryReport> GetSalesSummaryAsync(DateTime fromDate, DateTime toDate)
        {
            if (fromDate.Date > toDate.Date)
            {
                throw new ArgumentException(
                    "The start date cannot be after the end date.");
            }

            return await _repository.GetSalesSummaryAsync(
                fromDate.Date,
                toDate.Date);
        }

        // Methode N°3
        public async Task<List<ProductSalesReportItem>> GetBestSellingProductsAsync( DateTime fromDate, DateTime toDate)
        {
            if (fromDate.Date > toDate.Date)
            {
                throw new ArgumentException(
                    "The start date cannot be after the end date.");
            }

            return await _repository.GetBestSellingProductsAsync(
                fromDate.Date,
                toDate.Date);
        }

        // Methode N°4
        public async Task<List<DailySalesReportItem>>  GetDailySalesReportAsync(DateTime fromDate, DateTime toDate)
        {
            if (fromDate.Date > toDate.Date)
            {
                throw new ArgumentException(
                    "The start date cannot be after the end date.");
            }

            return await _repository.GetDailySalesReportAsync(
                fromDate.Date,
                toDate.Date);
        }
    }
}