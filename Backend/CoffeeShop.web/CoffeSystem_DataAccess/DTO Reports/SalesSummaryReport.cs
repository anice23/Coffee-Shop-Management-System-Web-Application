using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeSystem_DataAccess.DTO_Reports
{
  
        public class SalesSummaryReport
        {
            public int TotalOrders { get; set; }

            public decimal TotalRevenue { get; set; }

            public decimal AverageOrderValue { get; set; }

            public string BestSellingProduct { get; set; } = "N/A";

            public int BestSellingProductQuantity { get; set; }
        }
    
}
