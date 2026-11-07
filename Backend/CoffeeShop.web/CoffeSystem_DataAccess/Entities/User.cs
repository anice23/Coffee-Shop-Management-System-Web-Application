using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CoffeSystem_DataAccess.Entities
{
    public class User
    {
        // Proprety
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = "Cashier"; // Admin or Cashier
        public bool IsActive { get; set; } = true;

        // bech nfar9ou bin system admin w ghiro
        public bool IsSystemUser { get; set; } = false;


        // Nagivation : casiher => orders

        public ICollection<Order> Orders { get; set; } = new List<Order>();


    }
}
