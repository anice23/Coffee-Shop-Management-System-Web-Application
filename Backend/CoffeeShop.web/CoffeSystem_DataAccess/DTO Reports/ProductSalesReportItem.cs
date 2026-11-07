using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeSystem_DataAccess.DTO_Reports
{
    public class ProductSalesReportItem
    {
      
            public string ProductName { get; set; } = string.Empty;

            public int TotalQuantity { get; set; }

            public decimal TotalRevenue { get; set; }
        
    }
}
