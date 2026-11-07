using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeSystem_DataAccess.Entities
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }

        // Foreign Keys
        public int OrderId { get; set; }
        public int ProductId { get; set; }

        // Navigation Properties
        public Order Order { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
