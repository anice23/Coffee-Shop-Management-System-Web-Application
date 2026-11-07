using System;

namespace CoffeSystem_DataAccess.Exceptions
{
    public class ProductNotFoundException : Exception
    {
        public ProductNotFoundException(string message)
            : base(message)
        {
        }
    }

    public class InsufficientStockException : Exception
    {
        public InsufficientStockException(string message)
            : base(message)
        {
        }
    }

    public class InvalidOrderException : Exception
    {
        public InvalidOrderException(string message)
            : base(message)
        {
        }
    }
}