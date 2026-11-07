using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeSystem_DataAccess.DTO_Reports
{
    public class SalesReportItem
    {
        public int OrderId { get; set; }

        public DateTime OrderDate { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }
    }
}
