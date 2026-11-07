using CoffeSystem_DataAccess.Entities;

namespace CoffeeShop.API.DTOs
{
    public class CafeSettingsDto
    {
        public int CafeSettingId { get; set; }
        public string CafeName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string ReceiptFooter { get; set; } = string.Empty;
        public int LowStockThreshold { get; set; }


        public static CafeSettingsDto FromEntity(CafeSetting settings)
        {
            return new CafeSettingsDto
            {
                CafeSettingId = settings.CafeSettingId,
                CafeName = settings.CafeName,
                Address = settings.Address,
                PhoneNumber = settings.PhoneNumber,
                ReceiptFooter = settings.ReceiptFooter,
                LowStockThreshold = settings.LowStockThreshold
            };
        }

    }
}