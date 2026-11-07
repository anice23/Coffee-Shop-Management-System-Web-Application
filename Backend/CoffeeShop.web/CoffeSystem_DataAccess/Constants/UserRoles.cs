using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeSystem_DataAccess.Constants
{
    public static class UserRoles
    {
        public const string Admin = "Admin";
        public const string Cashier = "Cashier";

        public static readonly List<string> AllowedRoles = new()
        {
            Admin,
            Cashier
        };
    }
        
}

   
