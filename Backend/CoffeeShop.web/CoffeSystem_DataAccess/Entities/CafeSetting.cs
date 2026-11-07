using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeSystem_DataAccess.Entities
{
    public class CafeSetting
    {
        public int CafeSettingId { get; set; }
        public string CafeName { get; set; } = "Coffee System";
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string ReceiptFooter { get; set; }
            = "Thank you for visiting!";
        public int LowStockThreshold { get; set; } = 5;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
