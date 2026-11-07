using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeSystem_DataAccess.Entities
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        // optional 
        public string? ImagePath { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; } = true;

        // foreign key
        public int CategoryId { get; set; }

        // Navigation : 1: kol product => Categori   2: kol product => orderItem
        public Category Category { get; set; } = null!;
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();






    }
}
